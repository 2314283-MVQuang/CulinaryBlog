import { NextResponse } from "next/server";
import { registerLocalUser } from "@/lib/local-users";
import { registerSchema } from "@/lib/validations/auth.schema";

export async function POST(req: Request) {
  try {
    const body = await req.json();
    const validated = registerSchema.safeParse(body);

    if (!validated.success) {
      return NextResponse.json(
        { error: validated.error.issues[0]?.message || "Dữ liệu không hợp lệ" },
        { status: 400 }
      );
    }

    const { fullName, email, userName, password } = validated.data;
    const user = registerLocalUser({ fullName, email, userName, password });

    return NextResponse.json(
      {
        success: true,
        message: "Đăng ký tài khoản thành công!",
        user: {
          id: user.id,
          email: user.email,
          displayName: user.displayName,
          userName: user.userName,
        },
      },
      { status: 201 }
    );
  } catch (err: any) {
    return NextResponse.json(
      { error: err.message || "Đăng ký thất bại, vui lòng thử lại." },
      { status: 400 }
    );
  }
}
