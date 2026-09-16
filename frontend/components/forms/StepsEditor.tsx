"use client";

import { useState } from "react";
import { Check, Pencil, Plus, Trash2, X } from "lucide-react";
import { Button } from "@/components/ui/Button";
import { Input, Textarea } from "@/components/ui/Input";
import { apiErrorMessage } from "@/lib/api-client";
import { useAddStep, useDeleteStep, useUpdateStep } from "@/hooks/useRecipeParts";
import { formatMinutes } from "@/lib/utils";
import type { RecipeStep } from "@/types/recipe";

/**
 * FR-RCP-010 — thêm / sửa / xoá từng bước thực hiện.
 *
 * KHÔNG có ô nhập số thứ tự: backend tự đánh số khi thêm và tự đánh số lại toàn bộ khi xoá, nên
 * để người dùng tự gõ chỉ tạo cơ hội đụng ràng buộc UNIQUE(RecipeId, StepNumber) trong database.
 */
export function StepsEditor({ recipeId, steps }: { recipeId: string; steps: RecipeStep[] }) {
  const addStep = useAddStep(recipeId);
  const [error, setError] = useState<string | null>(null);
  const [draft, setDraft] = useState({ title: "", description: "", timerMinutes: "" });

  const sorted = steps.slice().sort((a, b) => a.stepNumber - b.stepNumber);

  async function handleAdd() {
    if (!draft.title.trim() || !draft.description.trim()) {
      setError("Bước thực hiện cần có cả tiêu đề lẫn mô tả.");
      return;
    }
    setError(null);
    try {
      await addStep.mutateAsync({
        title: draft.title.trim(),
        description: draft.description.trim(),
        timerMinutes: draft.timerMinutes ? Number(draft.timerMinutes) : null,
      });
      setDraft({ title: "", description: "", timerMinutes: "" });
    } catch (err) {
      setError(apiErrorMessage(err, "Không thêm được bước."));
    }
  }

  return (
    <div className="flex flex-col gap-3">
      {sorted.length === 0 && <p className="text-sm text-neutral-500">Chưa có bước nào.</p>}

      <ol className="flex flex-col gap-3">
        {sorted.map((step) => (
          <StepRow key={step.id} recipeId={recipeId} step={step} />
        ))}
      </ol>

      <div className="rounded-card border border-dashed border-neutral-300 p-3">
        <div className="flex flex-col gap-2">
          <div className="grid gap-2 sm:grid-cols-[3fr_1fr]">
            <Input
              placeholder="Tiêu đề bước (vd. Ninh xương)"
              value={draft.title}
              onChange={(e) => setDraft({ ...draft, title: e.target.value })}
            />
            <Input
              type="number"
              min={0}
              placeholder="Số phút"
              value={draft.timerMinutes}
              onChange={(e) => setDraft({ ...draft, timerMinutes: e.target.value })}
            />
          </div>
          <Textarea
            rows={2}
            placeholder="Mô tả chi tiết cách làm"
            value={draft.description}
            onChange={(e) => setDraft({ ...draft, description: e.target.value })}
          />
        </div>

        {error && <p className="mt-2 text-sm text-spice-600">{error}</p>}

        <div className="mt-2 flex justify-end">
          <Button size="sm" variant="outline" isLoading={addStep.isPending} onClick={handleAdd}>
            <Plus className="h-4 w-4" />
            Thêm bước
          </Button>
        </div>
      </div>
    </div>
  );
}

function StepRow({ recipeId, step }: { recipeId: string; step: RecipeStep }) {
  const updateStep = useUpdateStep(recipeId);
  const deleteStep = useDeleteStep(recipeId);

  const [isEditing, setIsEditing] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [draft, setDraft] = useState({
    title: step.title,
    description: step.description,
    timerMinutes: step.timerMinutes?.toString() ?? "",
  });

  async function handleSave() {
    setError(null);
    try {
      await updateStep.mutateAsync({
        stepId: step.id,
        input: {
          title: draft.title.trim(),
          description: draft.description.trim(),
          timerMinutes: draft.timerMinutes ? Number(draft.timerMinutes) : null,
        },
      });
      setIsEditing(false);
    } catch (err) {
      setError(apiErrorMessage(err, "Không lưu được bước."));
    }
  }

  async function handleDelete() {
    if (!confirm(`Xoá bước ${step.stepNumber} — "${step.title}"? Các bước sau sẽ được đánh số lại.`)) {
      return;
    }
    setError(null);
    try {
      await deleteStep.mutateAsync(step.id);
    } catch (err) {
      setError(apiErrorMessage(err, "Không xoá được bước."));
    }
  }

  return (
    <li className="flex gap-3">
      <div className="flex h-8 w-8 shrink-0 items-center justify-center rounded-full bg-brand-600 text-sm font-semibold text-white">
        {step.stepNumber}
      </div>

      <div className="flex-1">
        {isEditing ? (
          <div className="flex flex-col gap-2">
            <div className="grid gap-2 sm:grid-cols-[3fr_1fr]">
              <Input
                value={draft.title}
                onChange={(e) => setDraft({ ...draft, title: e.target.value })}
              />
              <Input
                type="number"
                min={0}
                placeholder="Số phút"
                value={draft.timerMinutes}
                onChange={(e) => setDraft({ ...draft, timerMinutes: e.target.value })}
              />
            </div>
            <Textarea
              rows={2}
              value={draft.description}
              onChange={(e) => setDraft({ ...draft, description: e.target.value })}
            />

            {error && <p className="text-sm text-spice-600">{error}</p>}

            <div className="flex justify-end gap-2">
              <Button size="sm" variant="ghost" onClick={() => setIsEditing(false)}>
                <X className="h-4 w-4" />
                Huỷ
              </Button>
              <Button size="sm" isLoading={updateStep.isPending} onClick={handleSave}>
                <Check className="h-4 w-4" />
                Lưu
              </Button>
            </div>
          </div>
        ) : (
          <>
            <div className="flex items-center justify-between gap-2">
              <h4 className="font-medium text-neutral-900">{step.title}</h4>
              <div className="flex shrink-0 items-center gap-1">
                {step.timerMinutes != null && (
                  <span className="whitespace-nowrap text-xs text-neutral-500">
                    {formatMinutes(step.timerMinutes)}
                  </span>
                )}
                <button
                  type="button"
                  aria-label={`Sửa bước ${step.stepNumber}`}
                  className="rounded-lg p-1.5 text-neutral-500 hover:bg-brand-50 hover:text-brand-700"
                  onClick={() => setIsEditing(true)}
                >
                  <Pencil className="h-4 w-4" />
                </button>
                <button
                  type="button"
                  aria-label={`Xoá bước ${step.stepNumber}`}
                  disabled={deleteStep.isPending}
                  className="rounded-lg p-1.5 text-neutral-500 hover:bg-spice-50 hover:text-spice-600 disabled:opacity-50"
                  onClick={handleDelete}
                >
                  <Trash2 className="h-4 w-4" />
                </button>
              </div>
            </div>
            <p className="mt-1 text-sm text-neutral-600">{step.description}</p>
            {error && <p className="mt-1 text-sm text-spice-600">{error}</p>}
          </>
        )}
      </div>
    </li>
  );
}
