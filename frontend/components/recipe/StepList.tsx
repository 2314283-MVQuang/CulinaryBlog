import Image from "next/image";
import { formatMinutes } from "@/lib/utils";
import type { RecipeStep } from "@/types/recipe";

/** Danh sách các bước thực hiện, hiển thị ở trang chi tiết recipe (mục 7.3, FR-RCP-010). */
export function StepList({ steps }: { steps: RecipeStep[] }) {
  if (steps.length === 0) {
    return <p className="text-sm text-neutral-500">Chưa có bước thực hiện nào.</p>;
  }

  const sorted = steps.slice().sort((a, b) => a.stepNumber - b.stepNumber);

  return (
    <ol className="flex flex-col gap-6">
      {sorted.map((step) => (
        <li key={step.id} className="flex gap-4">
          <div className="flex h-8 w-8 shrink-0 items-center justify-center rounded-full bg-brand-600 text-sm font-semibold text-white">
            {step.stepNumber}
          </div>
          <div className="flex-1">
            <div className="flex items-center justify-between gap-2">
              <h4 className="font-medium text-neutral-900">{step.title}</h4>
              {step.timerMinutes != null && (
                <span className="whitespace-nowrap text-xs text-neutral-500">{formatMinutes(step.timerMinutes)}</span>
              )}
            </div>
            <p className="mt-1 text-sm text-neutral-600">{step.description}</p>
            {step.imageUrl && (
              <div className="relative mt-3 aspect-video w-full max-w-md overflow-hidden rounded-lg">
                <Image src={step.imageUrl} alt={step.title} fill className="object-cover" />
              </div>
            )}
          </div>
        </li>
      ))}
    </ol>
  );
}
