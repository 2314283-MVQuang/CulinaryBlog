import { cn } from "@/lib/utils";

/** Spinner loading dùng chung — hiển thị khi TanStack Query đang `isLoading`. */
export function Spinner({ className }: { className?: string }) {
  return (
    <div
      role="status"
      aria-label="Đang tải"
      className={cn(
        "h-6 w-6 animate-spin rounded-full border-2 border-brand-200 border-t-brand-600",
        className,
      )}
    />
  );
}

/** Khối loading full-width, dùng để thay cả 1 vùng nội dung trong khi chờ dữ liệu. */
export function LoadingBlock({ label = "Đang tải dữ liệu..." }: { label?: string }) {
  return (
    <div className="flex flex-col items-center justify-center gap-3 py-16 text-neutral-500">
      <Spinner className="h-8 w-8" />
      <p className="text-sm">{label}</p>
    </div>
  );
}

/** Khối báo lỗi dùng chung khi query thất bại — có nút "Thử lại" tuỳ chọn. */
export function ErrorBlock({
  message = "Đã có lỗi xảy ra, vui lòng thử lại.",
  onRetry,
}: {
  message?: string;
  onRetry?: () => void;
}) {
  return (
    <div className="flex flex-col items-center justify-center gap-3 rounded-card border border-red-200 bg-red-50 py-12 text-center">
      <p className="text-sm text-red-700">{message}</p>
      {onRetry && (
        <button onClick={onRetry} className="text-sm font-medium text-red-700 underline underline-offset-2">
          Thử lại
        </button>
      )}
    </div>
  );
}
