import http from 'node:http';
import path from 'node:path';
import type { IncomingMessage, ServerResponse } from 'node:http';
import { defineConfig, type Connect, type Plugin } from 'vite';
import react from '@vitejs/plugin-react';
import tailwindcss from '@tailwindcss/vite';

const HOP_BY_HOP = new Set([
  'connection',
  'keep-alive',
  'proxy-authenticate',
  'proxy-authorization',
  'te',
  'trailers',
  'transfer-encoding',
  'upgrade',
  'content-length',
]);

function isApiPath(url: string | undefined): boolean {
  return Boolean(url?.startsWith('/api') || url?.startsWith('/health'));
}

function writeJson(res: ServerResponse, status: number, body: unknown) {
  const payload = Buffer.from(JSON.stringify(body));
  res.writeHead(status, {
    'content-type': 'application/json; charset=utf-8',
    'content-length': payload.length,
    'access-control-allow-origin': '*',
    'access-control-allow-private-network': 'true',
  });
  res.end(payload);
}

/**
 * Cursor's port forward resets chunked responses that also send Connection: close.
 * The HTML page loads (Vite sends Content-Length) but login POST looks like a
 * dropped connection, and the form shows "Giriş yapılamadı".
 * Buffer the API response and send a single Content-Length body.
 */
function bufferedApiProxy(): Plugin {
  const middleware: Connect.NextHandleFunction = (req, res, next) => {
    if (!isApiPath(req.url)) {
      next();
      return;
    }

    const origin = req.headers.origin ?? '*';
    if (req.method === 'OPTIONS') {
      res.writeHead(204, {
        'access-control-allow-origin': origin,
        'access-control-allow-methods': 'GET,POST,PUT,PATCH,DELETE,OPTIONS',
        'access-control-allow-headers': req.headers['access-control-request-headers'] ?? 'content-type, authorization, accept',
        'access-control-allow-private-network': 'true',
        'access-control-max-age': '600',
        vary: 'Origin, Access-Control-Request-Headers',
        'content-length': '0',
      });
      res.end();
      return;
    }

    const chunks: Buffer[] = [];
    req.on('data', (chunk: Buffer) => chunks.push(chunk));
    req.on('error', () => {
      if (!res.headersSent) writeJson(res, 400, { success: false, message: 'İstek okunamadı' });
    });
    req.on('end', () => {
      const body = Buffer.concat(chunks);
      const headers: http.OutgoingHttpHeaders = { ...req.headers, host: '127.0.0.1:5080' };
      delete headers.connection;
      delete headers['transfer-encoding'];
      headers['content-length'] = body.length;

      const upstream = http.request(
        {
          hostname: '127.0.0.1',
          port: 5080,
          method: req.method,
          path: req.url,
          headers,
        },
        (upstreamRes: IncomingMessage) => {
          const out: Buffer[] = [];
          upstreamRes.on('data', (chunk: Buffer) => out.push(chunk));
          upstreamRes.on('end', () => {
            const payload = Buffer.concat(out);
            const responseHeaders: http.OutgoingHttpHeaders = {};
            for (const [key, value] of Object.entries(upstreamRes.headers)) {
              if (value === undefined || HOP_BY_HOP.has(key.toLowerCase())) continue;
              responseHeaders[key] = value;
            }
            responseHeaders['content-length'] = payload.length;
            responseHeaders['access-control-allow-origin'] = origin;
            responseHeaders['access-control-allow-private-network'] = 'true';
            responseHeaders.vary = 'Origin';
            res.writeHead(upstreamRes.statusCode ?? 502, responseHeaders);
            res.end(payload);
          });
        },
      );
      upstream.on('error', () => {
        if (!res.headersSent) {
          writeJson(res, 502, { success: false, message: 'API ulaşılamadı' });
        }
      });
      upstream.end(body);
    });
  };

  return {
    name: 'naswood-buffered-api-proxy',
    configureServer(server) {
      server.middlewares.use(middleware);
    },
    configurePreviewServer(server) {
      server.middlewares.use(middleware);
    },
  };
}

export default defineConfig({
  plugins: [bufferedApiProxy(), react(), tailwindcss()],
  resolve: {
    alias: {
      '@': path.resolve(__dirname, './src'),
      '@naswood/ui': path.resolve(__dirname, '../../packages/ui/src'),
    },
  },
  server: {
    host: '0.0.0.0',
    port: 5173,
    strictPort: true,
    // Cursor / cloud port-forward previews send a non-localhost Host header.
    // Without this, Vite returns 403 and the preview stays blank/black.
    allowedHosts: true,
    // Vite 6 only reflects localhost origins. Cursor's preview host is rejected,
    // the browser drops the login call, and the form shows a connection error.
    cors: true,
    // HMR websocket through Cursor port proxy can fail and leave a blank frame.
    hmr: {
      overlay: true,
      clientPort: 5173,
    },
  },
  preview: {
    host: '0.0.0.0',
    port: 4173,
    allowedHosts: true,
    cors: true,
  },
});

