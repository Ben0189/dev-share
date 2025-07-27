import { z } from 'zod';

export const ShareResourceFormSchema = z.object({
  comment: z.string(),
  url: z
    .string()
    .url('Please enter a valid URL (including http:// or https://)')
    .refine((val) => /^https?:\/\//.test(val), {
      message: 'URL must start with http:// or https://',
    }),
});
