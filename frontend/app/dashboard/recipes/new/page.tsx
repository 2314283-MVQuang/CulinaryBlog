import { RecipeWizardForm } from "@/components/forms/RecipeWizardForm";

export const metadata = { title: "Tạo công thức mới" };

// Mục 9: "/dashboard/recipes/new" — CSR.
export default function NewRecipePage() {
  return (
    <div className="flex flex-col gap-6">
      <h1 className="text-2xl font-bold text-neutral-900">Tạo công thức mới</h1>
      <RecipeWizardForm />
    </div>
  );
}
