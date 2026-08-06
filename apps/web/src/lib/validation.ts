import { z } from 'zod';

/**
 * Shared validation helpers for upcoming forms (Login, Settings, …).
 * No business schemas yet — foundation only.
 */
export const nonEmptyString = z.string().trim().min(1, 'Required');

export const emailSchema = z.string().trim().email('Invalid email');
