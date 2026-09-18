import fs from "node:fs";
import path from "node:path";

export interface LocalUser {
  id: string;
  email: string;
  password: string;
  displayName: string;
  userName: string;
  roles: string[];
  avatarUrl?: string;
  createdAt: string;
}

const DATA_DIR = path.join(process.cwd(), "data");
const USERS_FILE = path.join(DATA_DIR, "users.json");

export const SEED_USERS: LocalUser[] = [
  {
    id: "11111111-1111-1111-1111-111111111111",
    email: "admin@culinaryblog.local",
    password: "Admin@123",
    displayName: "Quản trị viên (Admin)",
    userName: "admin",
    roles: ["Admin", "Author"],
    createdAt: "2026-09-01T00:00:00.000Z",
  },
  {
    id: "22222222-2222-2222-2222-222222222222",
    email: "author@culinaryblog.local",
    password: "Author@123",
    displayName: "Đầu bếp mẫu (Author)",
    userName: "author",
    roles: ["Author"],
    createdAt: "2026-09-01T00:00:00.000Z",
  },
];

function ensureStorage(): LocalUser[] {
  try {
    if (!fs.existsSync(DATA_DIR)) {
      fs.mkdirSync(DATA_DIR, { recursive: true });
    }
    if (!fs.existsSync(USERS_FILE)) {
      fs.writeFileSync(USERS_FILE, JSON.stringify(SEED_USERS, null, 2), "utf8");
      return SEED_USERS;
    }
    const raw = fs.readFileSync(USERS_FILE, "utf8");
    const users = JSON.parse(raw) as LocalUser[];
    return Array.isArray(users) && users.length > 0 ? users : SEED_USERS;
  } catch {
    return SEED_USERS;
  }
}

export function getAllUsers(): LocalUser[] {
  return ensureStorage();
}

export function findLocalUser(email: string, password?: string): LocalUser | null {
  const users = ensureStorage();
  const normalizedEmail = email.trim().toLowerCase();
  const user = users.find((u) => u.email.toLowerCase() === normalizedEmail);

  if (!user) return null;
  if (password && user.password !== password) return null;

  return user;
}

export function registerLocalUser(userData: {
  fullName: string;
  email: string;
  userName: string;
  password: string;
}): LocalUser {
  const users = ensureStorage();
  const normalizedEmail = userData.email.trim().toLowerCase();

  const existing = users.find(
    (u) => u.email.toLowerCase() === normalizedEmail || u.userName.toLowerCase() === userData.userName.toLowerCase()
  );

  if (existing) {
    if (existing.email.toLowerCase() === normalizedEmail) {
      throw new Error("Email này đã được đăng ký.");
    }
    throw new Error("Tên đăng nhập này đã được sử dụng.");
  }

  const newUser: LocalUser = {
    id: crypto.randomUUID(),
    email: userData.email.trim(),
    password: userData.password,
    displayName: userData.fullName.trim(),
    userName: userData.userName.trim(),
    roles: ["Author"],
    createdAt: new Date().toISOString(),
  };

  users.push(newUser);

  try {
    fs.writeFileSync(USERS_FILE, JSON.stringify(users, null, 2), "utf8");
  } catch (err) {
    console.error("Lỗi lưu file users.json:", err);
  }

  return newUser;
}
