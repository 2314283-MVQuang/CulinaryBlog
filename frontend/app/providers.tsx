"use client";

import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { ReactQueryDevtools } from "@tanstack/react-query-devtools";
import { SessionProvider } from "next-auth/react";
import { useState } from "react";

/**
 * Gom toàn bộ Context Provider của app vào 1 chỗ — app/layout.tsx (Server Component)
 * không thể tự dùng Provider (chúng cần "use client"), nên phải bọc qua component này.
 */
export function Providers({ children }: { children: React.ReactNode }) {
  // Tạo QueryClient bằng useState (không phải useMemo) theo đúng khuyến nghị của TanStack Query
  // cho App Router, đảm bảo mỗi request ở server chỉ tạo 1 instance, tránh rò rỉ cache giữa các user.
  const [queryClient] = useState(
    () =>
      new QueryClient({
        defaultOptions: {
          queries: {
            staleTime: 30_000,
            retry: 1,
          },
        },
      }),
  );

  return (
    <SessionProvider>
      <QueryClientProvider client={queryClient}>
        {children}
        {process.env.NODE_ENV === "development" && <ReactQueryDevtools initialIsOpen={false} />}
      </QueryClientProvider>
    </SessionProvider>
  );
}
