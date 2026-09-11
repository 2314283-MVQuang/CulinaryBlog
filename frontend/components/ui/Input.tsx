import { type InputHTMLAttributes, type TextareaHTMLAttributes, forwardRef } from "react";
import { cn } from "@/lib/utils";

interface FieldWrapperProps {
  label?: string;
  error?: string;
  hint?: string;
}

/**
 * Style nền của mọi ô nhập (input/textarea/select).
 * `focus:shadow-glow` tạo quầng sáng cam quanh ô đang gõ — dấu hiệu rõ ràng hơn viền mảnh,
 * đặc biệt hữu ích trên form dài như trình soạn công thức.
 */
const fieldBaseClass =
  "w-full rounded-xl border border-neutral-200 bg-white px-3.5 py-2.5 text-sm text-neutral-900 " +
  "transition-all duration-300 ease-springy placeholder:text-neutral-400 " +
  "hover:border-brand-200 focus:border-brand-400 focus:shadow-glow focus:outline-none " +
  "disabled:cursor-not-allowed disabled:bg-neutral-100 disabled:text-neutral-500";

/** Style khi field có lỗi validate — đổi sang tông đỏ ớt của theme. */
const fieldErrorClass = "border-spice-400 focus:border-spice-500 focus:shadow-none focus:ring-2 focus:ring-spice-300";

export interface InputProps extends InputHTMLAttributes<HTMLInputElement>, FieldWrapperProps {}

/**
 * Input dùng chung, tích hợp sẵn hiển thị `label` + `error` — dùng với react-hook-form:
 *   <Input label="Email" error={errors.email?.message} {...register("email")} />
 */
export const Input = forwardRef<HTMLInputElement, InputProps>(
  ({ className, label, error, hint, id, ...props }, ref) => {
    const inputId = id ?? props.name;
    return (
      <div className="flex flex-col gap-1">
        {label && (
          <label htmlFor={inputId} className="text-sm font-medium text-neutral-700">
            {label}
          </label>
        )}
        <input
          ref={ref}
          id={inputId}
          className={cn(fieldBaseClass, error && fieldErrorClass, className)}
          aria-invalid={!!error}
          {...props}
        />
        {hint && !error && <p className="text-xs text-neutral-500">{hint}</p>}
        {error && <p className="text-xs font-medium text-spice-600">{error}</p>}
      </div>
    );
  },
);
Input.displayName = "Input";

export interface TextareaProps extends TextareaHTMLAttributes<HTMLTextAreaElement>, FieldWrapperProps {}

export const Textarea = forwardRef<HTMLTextAreaElement, TextareaProps>(
  ({ className, label, error, hint, id, rows = 4, ...props }, ref) => {
    const inputId = id ?? props.name;
    return (
      <div className="flex flex-col gap-1">
        {label && (
          <label htmlFor={inputId} className="text-sm font-medium text-neutral-700">
            {label}
          </label>
        )}
        <textarea
          ref={ref}
          id={inputId}
          rows={rows}
          className={cn(fieldBaseClass, "resize-y", error && fieldErrorClass, className)}
          aria-invalid={!!error}
          {...props}
        />
        {hint && !error && <p className="text-xs text-neutral-500">{hint}</p>}
        {error && <p className="text-xs font-medium text-spice-600">{error}</p>}
      </div>
    );
  },
);
Textarea.displayName = "Textarea";

export interface SelectOption {
  value: string;
  label: string;
}

export interface SelectProps extends Omit<InputHTMLAttributes<HTMLSelectElement>, "size">, FieldWrapperProps {
  options: SelectOption[];
  placeholder?: string;
}

export const Select = forwardRef<HTMLSelectElement, SelectProps>(
  ({ className, label, error, options, placeholder, id, ...props }, ref) => {
    const inputId = id ?? props.name;
    return (
      <div className="flex flex-col gap-1">
        {label && (
          <label htmlFor={inputId} className="text-sm font-medium text-neutral-700">
            {label}
          </label>
        )}
        <select
          ref={ref}
          id={inputId}
          className={cn(fieldBaseClass, "bg-white", error && fieldErrorClass, className)}
          aria-invalid={!!error}
          {...props}
        >
          {placeholder && <option value="">{placeholder}</option>}
          {options.map((opt) => (
            <option key={opt.value} value={opt.value}>
              {opt.label}
            </option>
          ))}
        </select>
        {error && <p className="text-xs font-medium text-spice-600">{error}</p>}
      </div>
    );
  },
);
Select.displayName = "Select";
