import { BookOpen, ChefHat, Clock, Heart } from "lucide-react";
import { CountUp } from "@/components/ui/CountUp";
import { Reveal } from "@/components/ui/Reveal";

interface StatsBarProps {
  recipeCount: number;
  categoryCount: number;
}

/**
 * Dải số liệu dưới hero. Con số chạy từ 0 lên khi người dùng cuộn tới (component CountUp).
 *
 * `recipeCount` / `categoryCount` lấy từ API thật; 2 chỉ số còn lại là số minh hoạ cố định —
 * khi backend có endpoint thống kê (mục 8) thì thay bằng dữ liệu thật.
 */
export function StatsBar({ recipeCount, categoryCount }: StatsBarProps) {
  const stats = [
    { icon: BookOpen, value: recipeCount, suffix: "+", label: "Công thức" },
    { icon: ChefHat, value: categoryCount, suffix: "", label: "Danh mục món" },
    { icon: Clock, value: 15, suffix: " phút", label: "Món nhanh nhất" },
    { icon: Heart, value: 1280, suffix: "+", label: "Lượt yêu thích" },
  ];

  return (
    <section className="container-page -mt-6 sm:-mt-10">
      <Reveal>
        <div className="grid grid-cols-2 gap-px overflow-hidden rounded-card-lg bg-brand-100 shadow-soft lg:grid-cols-4">
          {stats.map(({ icon: Icon, value, suffix, label }, index) => (
            <div
              key={label}
              className="group flex items-center gap-3 bg-white px-5 py-6 transition-colors duration-300 hover:bg-brand-50 sm:gap-4 sm:px-6"
            >
              <span className="flex h-11 w-11 shrink-0 items-center justify-center rounded-xl bg-brand-gradient text-white shadow-soft transition-transform duration-300 ease-springy group-hover:scale-110 group-hover:rotate-6">
                <Icon className="h-5 w-5" />
              </span>
              <span className="min-w-0">
                <span className="block text-xl font-bold text-neutral-900 sm:text-2xl">
                  <CountUp to={value} suffix={suffix} duration={1400 + index * 150} />
                </span>
                <span className="block truncate text-xs text-neutral-500 sm:text-sm">{label}</span>
              </span>
            </div>
          ))}
        </div>
      </Reveal>
    </section>
  );
}
