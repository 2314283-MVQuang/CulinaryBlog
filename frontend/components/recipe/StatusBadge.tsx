import { Badge } from "@/components/ui/Badge";
import { STATUS_LABEL } from "@/lib/utils";
import type { RecipeStatus } from "@/types/recipe";

/** Badge hiển thị Draft/Published/Archived — chỉ dùng trong dashboard, trang public không hiện. */
export function StatusBadge({ status }: { status: RecipeStatus }) {
  const info = STATUS_LABEL[status] ?? STATUS_LABEL.Draft;
  return <Badge className={info?.className}>{info?.label}</Badge>;
}
