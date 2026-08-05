import * as React from 'react';
import { cva, type VariantProps } from 'class-variance-authority';
import { cn } from '../lib/cn';

const buttonVariants = cva(
  'inline-flex items-center justify-center gap-2 whitespace-nowrap rounded-[var(--radius-md)] text-sm font-medium transition-colors focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-[var(--border-focus)] disabled:pointer-events-none disabled:opacity-50',
  {
    variants: {
      variant: {
        default:
          'bg-[var(--color-primary)] text-[var(--text-inverse)] hover:bg-[var(--color-primary-hover)] active:bg-[var(--color-primary-active)]',
        secondary:
          'bg-[var(--color-secondary)] text-[var(--text-inverse)] hover:bg-[var(--color-secondary-hover)]',
        outline:
          'border border-[var(--border-default)] bg-transparent text-[var(--text-primary)] hover:bg-[var(--color-surface-hover)]',
        ghost: 'text-[var(--text-primary)] hover:bg-[var(--color-surface-hover)]',
        danger: 'bg-[var(--color-danger)] text-[var(--text-inverse)] hover:opacity-90',
      },
      size: {
        sm: 'h-8 px-3',
        md: 'h-10 px-4',
        lg: 'h-11 px-6',
      },
    },
    defaultVariants: {
      variant: 'default',
      size: 'md',
    },
  },
);

export interface ButtonProps
  extends React.ButtonHTMLAttributes<HTMLButtonElement>,
    VariantProps<typeof buttonVariants> {}

export const Button = React.forwardRef<HTMLButtonElement, ButtonProps>(
  ({ className, variant, size, type = 'button', ...props }, ref) => (
    <button
      ref={ref}
      type={type}
      className={cn(buttonVariants({ variant, size }), className)}
      {...props}
    />
  ),
);
Button.displayName = 'Button';

export { buttonVariants };
