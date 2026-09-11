-- =============================================================================
--  01 — EXTENSIONS & CẤU HÌNH FULL-TEXT SEARCH TIẾNG VIỆT
--  Chạy đầu tiên. Các file trong thư mục này được PostgreSQL chạy theo thứ tự tên.
-- =============================================================================

-- gen_random_uuid() + gen_random_bytes() dùng cho khoá chính UUID và cột RowVersion.
-- (PostgreSQL 13+ đã có sẵn gen_random_uuid(), nhưng gen_random_bytes() thì vẫn cần pgcrypto.)
CREATE EXTENSION IF NOT EXISTS pgcrypto;

-- unaccent: bỏ dấu tiếng Việt để tìm "pho bo" vẫn ra "phở bò" (mục 7.2, FR-SRCH-001).
CREATE EXTENSION IF NOT EXISTS unaccent;

-- pg_trgm: index trigram cho cột Title, hỗ trợ tìm gần đúng / gõ sai chính tả.
CREATE EXTENSION IF NOT EXISTS pg_trgm;

-- -----------------------------------------------------------------------------
-- Cấu hình tìm kiếm "vietnamese"
--
-- PostgreSQL không có bộ tách từ tiếng Việt sẵn. Cách làm thực tế và đủ dùng:
-- lấy cấu hình 'simple' (tách theo khoảng trắng, không cắt gốc từ kiểu tiếng Anh)
-- rồi thêm bước unaccent để bỏ dấu. Nhờ vậy "Phở Bò", "pho bo", "PHO BO" đều
-- khớp nhau khi tìm kiếm.
-- -----------------------------------------------------------------------------
DO $$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM pg_ts_config WHERE cfgname = 'vietnamese') THEN
        CREATE TEXT SEARCH CONFIGURATION vietnamese (COPY = simple);

        ALTER TEXT SEARCH CONFIGURATION vietnamese
            ALTER MAPPING FOR hword, hword_part, word
            WITH unaccent, simple;
    END IF;
END
$$;
