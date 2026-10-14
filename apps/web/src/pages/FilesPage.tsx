import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { useRef, useState } from 'react';
import { Button, Card, CardContent, CardDescription, CardHeader, CardTitle, Input } from '@naswood/ui';
import { ApiClientError } from '@/api/types';
import { deleteFile, downloadFile, searchFiles, uploadFile } from '@/api/files';

function formatBytes(size: number): string {
  if (size < 1024) return `${size} B`;
  if (size < 1024 * 1024) return `${(size / 1024).toFixed(1)} KB`;
  return `${(size / (1024 * 1024)).toFixed(1)} MB`;
}

export function FilesPage() {
  const queryClient = useQueryClient();
  const inputRef = useRef<HTMLInputElement>(null);
  const [search, setSearch] = useState('');
  const [queryName, setQueryName] = useState('');
  const [message, setMessage] = useState<string | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [dragOver, setDragOver] = useState(false);

  const listQuery = useQuery({
    queryKey: ['files', queryName],
    queryFn: () => searchFiles({ name: queryName || undefined }),
  });

  const uploadMutation = useMutation({
    mutationFn: (file: File) =>
      uploadFile(file, { module: 'Platform', category: 'General', tags: 'upload' }),
    onSuccess: async (file) => {
      setError(null);
      setMessage(`Uploaded ${file.originalName}`);
      await queryClient.invalidateQueries({ queryKey: ['files'] });
    },
    onError: (err) => {
      setMessage(null);
      setError(err instanceof ApiClientError ? err.message : 'Upload failed.');
    },
  });

  const deleteMutation = useMutation({
    mutationFn: deleteFile,
    onSuccess: async () => {
      setError(null);
      setMessage('File deleted.');
      await queryClient.invalidateQueries({ queryKey: ['files'] });
    },
    onError: (err) => {
      setMessage(null);
      setError(err instanceof ApiClientError ? err.message : 'Delete failed.');
    },
  });

  const onFiles = async (files: FileList | File[]) => {
    const list = Array.from(files);
    for (const file of list) {
      await uploadMutation.mutateAsync(file);
    }
  };

  return (
    <section className="space-y-6">
      <div>
        <h1 className="text-2xl font-semibold tracking-tight">Files</h1>
        <p className="mt-1 text-[var(--text-secondary)]">
          Upload, search, download and delete platform files (Local storage provider).
        </p>
      </div>

      <Card>
        <CardHeader>
          <CardTitle>Upload</CardTitle>
          <CardDescription>Drag and drop or choose files. Allowed: pdf, images, txt, csv, office, zip.</CardDescription>
        </CardHeader>
        <CardContent className="space-y-4">
          <div
            className={`flex min-h-36 cursor-pointer flex-col items-center justify-center rounded-[var(--radius-lg)] border-2 border-dashed px-4 text-center transition-colors ${
              dragOver
                ? 'border-[var(--color-primary)] bg-[var(--color-primary)]/10'
                : 'border-[var(--border-default)] bg-[var(--color-surface)]'
            }`}
            onDragOver={(event) => {
              event.preventDefault();
              setDragOver(true);
            }}
            onDragLeave={() => setDragOver(false)}
            onDrop={(event) => {
              event.preventDefault();
              setDragOver(false);
              if (event.dataTransfer.files.length) {
                void onFiles(event.dataTransfer.files);
              }
            }}
            onClick={() => inputRef.current?.click()}
          >
            <p className="text-sm font-medium">Drop files here</p>
            <p className="mt-1 text-xs text-[var(--text-muted)]">or click to browse</p>
            <input
              ref={inputRef}
              type="file"
              className="hidden"
              multiple
              onChange={(event) => {
                if (event.target.files?.length) {
                  void onFiles(event.target.files);
                  event.target.value = '';
                }
              }}
            />
          </div>
          {uploadMutation.isPending ? (
            <p className="text-sm text-[var(--text-secondary)]">Uploading…</p>
          ) : null}
          {message ? <p className="text-sm text-[var(--color-success)]">{message}</p> : null}
          {error ? <p className="text-sm text-[var(--color-danger)]">{error}</p> : null}
        </CardContent>
      </Card>

      <Card>
        <CardHeader>
          <CardTitle>Library</CardTitle>
          <CardDescription>Current versions only.</CardDescription>
        </CardHeader>
        <CardContent className="space-y-4">
          <form
            className="flex flex-wrap gap-2"
            onSubmit={(event) => {
              event.preventDefault();
              setQueryName(search.trim());
            }}
          >
            <Input
              value={search}
              onChange={(event) => setSearch(event.target.value)}
              placeholder="Search by name or number"
              className="max-w-sm"
            />
            <Button type="submit" variant="secondary">
              Search
            </Button>
          </form>

          {listQuery.isLoading ? (
            <p className="text-sm text-[var(--text-secondary)]">Loading files…</p>
          ) : listQuery.isError ? (
            <p className="text-sm text-[var(--color-danger)]">Failed to load files.</p>
          ) : (listQuery.data?.items.length ?? 0) === 0 ? (
            <p className="text-sm text-[var(--text-muted)]">No files yet.</p>
          ) : (
            <div className="overflow-x-auto">
              <table className="w-full min-w-[640px] border-collapse text-left text-sm">
                <thead>
                  <tr className="border-b border-[var(--border-default)] text-[var(--text-muted)]">
                    <th className="px-2 py-2 font-medium">Number</th>
                    <th className="px-2 py-2 font-medium">Name</th>
                    <th className="px-2 py-2 font-medium">Size</th>
                    <th className="px-2 py-2 font-medium">Module</th>
                    <th className="px-2 py-2 font-medium">Uploaded</th>
                    <th className="px-2 py-2 font-medium">Actions</th>
                  </tr>
                </thead>
                <tbody>
                  {listQuery.data!.items.map((file) => (
                    <tr key={file.id} className="border-b border-[var(--border-default)]">
                      <td className="px-2 py-2 font-mono text-xs">{file.number}</td>
                      <td className="px-2 py-2">
                        <div className="font-medium">{file.originalName}</div>
                        <div className="text-xs text-[var(--text-muted)]">
                          v{file.version} · {file.contentType}
                        </div>
                      </td>
                      <td className="px-2 py-2">{formatBytes(file.sizeBytes)}</td>
                      <td className="px-2 py-2">{file.module}</td>
                      <td className="px-2 py-2 text-xs">
                        {new Date(file.uploadedAt).toLocaleString()}
                      </td>
                      <td className="px-2 py-2">
                        <div className="flex flex-wrap gap-2">
                          <Button
                            type="button"
                            size="sm"
                            variant="outline"
                            onClick={() => void downloadFile(file.id, file.originalName)}
                          >
                            Download
                          </Button>
                          <Button
                            type="button"
                            size="sm"
                            variant="danger"
                            disabled={deleteMutation.isPending}
                            onClick={() => deleteMutation.mutate(file.id)}
                          >
                            Delete
                          </Button>
                        </div>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          )}
        </CardContent>
      </Card>
    </section>
  );
}
