"use client";

import { ProfileForm } from "@/components/forms/ProfileForm";
import { ErrorBlock, LoadingBlock } from "@/components/ui/Spinner";
import { useMyProfile } from "@/hooks/useProfile";

// Mục 9: "/profile" — CSR, yêu cầu đăng nhập (middleware.ts đã chặn ở matcher "/profile/:path*").
export default function ProfilePage() {
  const { data: profile, isLoading, isError, refetch } = useMyProfile();

  return (
    <div className="container-page flex justify-center py-10">
      <div className="w-full max-w-lg">
        <h1 className="mb-6 text-2xl font-bold text-neutral-900">Hồ sơ cá nhân</h1>

        {isLoading && <LoadingBlock />}
        {isError && <ErrorBlock message="Không tải được hồ sơ." onRetry={() => refetch()} />}
        {profile && <ProfileForm user={profile} />}
      </div>
    </div>
  );
}
