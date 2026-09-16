# =============================================================================
#  Kiem tra 3 tinh nang backend cua Quang
#
#    FR-AUTH-007        : PATCH /auth/me
#    FR-RCP-005/006     : publish / unpublish / archive
#    FR-RCP-008/009/010 : CRUD anh, nguyen lieu, buoc
#
#  CACH DUNG
#    1. Cua so PowerShell thu nhat, de backend chay:
#           dotnet run --project src\CulinaryBlog.API --launch-profile http
#       Doi den khi thay dong "Now listening on: http://localhost:5000"
#       (Khong chay duoc thi xem muc "Loi hay gap" trong README.md o goc repo.)
#
#    2. Cua so PowerShell thu hai, chay script nay:
#           .\test-3-tinh-nang.ps1
#
#  Script tu tao mot tai khoan moi moi lan chay nen chay lai bao nhieu lan cung duoc.
# =============================================================================

$ErrorActionPreference = "Stop"
$api = "http://localhost:5000/api/v1"

# --- Bo dem ket qua -----------------------------------------------------------
$script:pass = 0
$script:fail = 0

function Check($ten, $dieuKien, $thucTe) {
    if ($dieuKien) {
        Write-Host "  [PASS] $ten" -ForegroundColor Green
        $script:pass++
    } else {
        Write-Host "  [FAIL] $ten" -ForegroundColor Red
        Write-Host "         thuc te: $thucTe" -ForegroundColor DarkGray
        $script:fail++
    }
}

function Muc($ten) {
    Write-Host ""
    Write-Host "== $ten" -ForegroundColor Cyan
}

# Goi API va tra ve ma loi thay vi nem exception, de test duoc cac ca 4xx.
function TryApi($method, $url, $headers, $body) {
    try {
        if ($null -eq $body) {
            $r = Invoke-RestMethod -Method $method -Uri $url -Headers $headers
        } else {
            $r = Invoke-RestMethod -Method $method -Uri $url -Headers $headers `
                                   -ContentType "application/json" -Body $body
        }
        return @{ ok = $true; status = 200; data = $r }
    } catch {
        $code = 0
        if ($_.Exception.Response) { $code = [int]$_.Exception.Response.StatusCode }
        return @{ ok = $false; status = $code; body = $_.ErrorDetails.Message }
    }
}

function Json($obj) { $obj | ConvertTo-Json -Depth 6 }

# =============================================================================
Muc "0. Kiem tra backend da chay chua"

try {
    $cats = Invoke-RestMethod "$api/categories"
    Check "Backend phan hoi tai $api" $true ""
} catch {
    Write-Host "  [FAIL] Khong ket noi duoc $api" -ForegroundColor Red
    Write-Host ""
    Write-Host "  Backend chua chay. Mo mot cua so PowerShell khac va chay:" -ForegroundColor Yellow
    Write-Host "      dotnet run --project src\CulinaryBlog.API --launch-profile http" -ForegroundColor Yellow
    exit 1
}

$catId = $cats.data[0].id
Check "Lay duoc danh muc mau tu database" ($null -ne $catId) "catId = $catId"

# =============================================================================
Muc "1. Dang ky tai khoan va lay JWT"

$suffix = Get-Random -Maximum 999999
$email  = "quang$suffix@test.local"

$reg = TryApi POST "$api/auth/register" $null (Json @{
    fullName = "Mai Van Quang"
    email    = $email
    userName = "quang$suffix"
    password = "Test@1234"
})
Check "POST /auth/register tao duoc tai khoan" $reg.ok "status $($reg.status) $($reg.body)"
if (-not $reg.ok) { Write-Host "Dung lai vi khong co token." -ForegroundColor Red; exit 1 }

$token = $reg.data.data.accessToken
$H = @{ Authorization = "Bearer $token" }
Check "Nhan duoc accessToken" ($token.Length -gt 20) "do dai = $($token.Length)"
Check "Tai khoan moi duoc gan role Author" ($reg.data.data.user.roles -contains "Author") "roles = $($reg.data.data.user.roles -join ',')"

# =============================================================================
Muc "2. FR-AUTH-007 - PATCH /auth/me"

$u1 = TryApi PATCH "$api/auth/me" $H (Json @{
    displayName = "Quang Mai"
    bio         = "Sinh vien DLU"
    avatarUrl   = "/uploads/avatar/quang.png"
})
Check "Cap nhat duoc ho so" $u1.ok "status $($u1.status) $($u1.body)"
Check "displayName da doi thanh 'Quang Mai'" ($u1.data.data.displayName -eq "Quang Mai") "$($u1.data.data.displayName)"
Check "bio da luu" ($u1.data.data.bio -eq "Sinh vien DLU") "$($u1.data.data.bio)"

# Ngu nghia PATCH: khong gui truong nao thi giu nguyen truong do.
$u2 = TryApi PATCH "$api/auth/me" $H (Json @{ bio = "Thich nau an" })
Check "Khong gui displayName thi GIU NGUYEN gia tri cu" ($u2.data.data.displayName -eq "Quang Mai") "$($u2.data.data.displayName)"
Check "bio doi duoc rieng le" ($u2.data.data.bio -eq "Thich nau an") "$($u2.data.data.bio)"

# Chuoi rong = xoa gia tri, khac han voi khong gui.
$u3 = TryApi PATCH "$api/auth/me" $H (Json @{ bio = "" })
Check "Gui chuoi rong thi XOA gia tri (bio = null)" ([string]::IsNullOrEmpty($u3.data.data.bio)) "bio = '$($u3.data.data.bio)'"
Check "displayName van con nguyen sau khi xoa bio" ($u3.data.data.displayName -eq "Quang Mai") "$($u3.data.data.displayName)"

# Khong duoc phep sua ho so nguoi khac: userId lay tu JWT nen body co gui gi cung vo nghia.
$u4 = TryApi PATCH "$api/auth/me" $H (Json @{ userId = "00000000-0000-0000-0000-000000000001"; displayName = "Ke gia mao" })
Check "Gui kem userId la khong co tac dung (van sua ho so cua minh)" ($u4.data.data.email -eq $email) "$($u4.data.data.email)"

# =============================================================================
Muc "3. Tao mot cong thuc de thu nghiem"

$rec = TryApi POST "$api/recipes" $H (Json @{
    title        = "Pho bo test $suffix"
    description  = "Cong thuc dung de kiem thu"
    instructions = "Ninh xuong, chan banh pho, chan nuoc dung"
    categoryId   = $catId
    prepTime     = 30
    cookTime     = 180
    servings     = 4
    difficulty   = "Medium"
})
Check "Tao duoc cong thuc" $rec.ok "status $($rec.status) $($rec.body)"
if (-not $rec.ok) { Write-Host "Dung lai." -ForegroundColor Red; exit 1 }

$rid  = $rec.data.data.id
$slug = $rec.data.data.slug
Check "Cong thuc moi o trang thai Draft" ($rec.data.data.status -eq "Draft") "$($rec.data.data.status)"

# =============================================================================
Muc "4. FR-RCP-005/006 - doi trang thai"

# Cong thuc rong ruot thi KHONG duoc publish.
$p1 = TryApi PATCH "$api/recipes/$rid/publish" $H $null
Check "Publish cong thuc chua co buoc/nguyen lieu bi tu choi 422" ($p1.status -eq 422) "status $($p1.status)"
Check "Loi 422 co chi ro thieu 'steps'" ($p1.body -match "steps") "$($p1.body)"
Check "Loi 422 co chi ro thieu 'ingredients'" ($p1.body -match "ingredients") "$($p1.body)"

# =============================================================================
Muc "5. FR-RCP-009 - nguyen lieu"

$ing1 = TryApi POST "$api/recipes/$rid/ingredients" $H (Json @{ name = "Banh pho"; quantity = 500; unit = "g" })
Check "Them duoc nguyen lieu" $ing1.ok "status $($ing1.status) $($ing1.body)"
$ingId = $ing1.data.data.id

$ing2 = TryApi PUT "$api/recipes/$rid/ingredients/$ingId" $H (Json @{ name = "Banh pho tuoi"; quantity = 600; unit = "g"; notes = "Mua loai soi to" })
Check "Sua duoc nguyen lieu" ($ing2.data.data.name -eq "Banh pho tuoi") "$($ing2.data.data.name)"
Check "Sua duoc so luong" ($ing2.data.data.quantity -eq 600) "$($ing2.data.data.quantity)"

# =============================================================================
Muc "6. FR-RCP-010 - buoc nau va viec danh so lai"

$s1 = TryApi POST "$api/recipes/$rid/steps" $H (Json @{ title = "Ninh xuong";    description = "Ninh 3 tieng"; timerMinutes = 180 })
$s2 = TryApi POST "$api/recipes/$rid/steps" $H (Json @{ title = "Chan banh pho"; description = "Chan nuoc soi" })
$s3 = TryApi POST "$api/recipes/$rid/steps" $H (Json @{ title = "Chan nuoc dung"; description = "Chan that nong" })
Check "Them duoc 3 buoc" ($s1.ok -and $s2.ok -and $s3.ok) "$($s1.status)/$($s2.status)/$($s3.status)"
Check "Buoc dau duoc danh so 1" ($s1.data.data.stepNumber -eq 1) "$($s1.data.data.stepNumber)"
Check "Buoc thu ba duoc danh so 3" ($s3.data.data.stepNumber -eq 3) "$($s3.data.data.stepNumber)"

# Xoa buoc GIUA - day la cho de sinh loi nhat vi rang buoc UNIQUE(RecipeId, StepNumber).
$del = TryApi DELETE "$api/recipes/$rid/steps/$($s2.data.data.id)" $H $null
Check "Xoa duoc buoc so 2 (buoc o giua)" $del.ok "status $($del.status) $($del.body)"

$sau = TryApi GET "$api/recipes/$slug" $H $null
$steps = $sau.data.data.steps | Sort-Object stepNumber
Check "Con lai dung 2 buoc" ($steps.Count -eq 2) "$($steps.Count) buoc"
Check "Cac buoc duoc danh so lai lien tuc 1,2" (($steps.stepNumber -join ",") -eq "1,2") "$($steps.stepNumber -join ',')"
Check "Buoc 2 bay gio la 'Chan nuoc dung'" ($steps[1].title -eq "Chan nuoc dung") "$($steps[1].title)"

# =============================================================================
Muc "7. FR-RCP-005/006 - publish sau khi da du dieu kien"

$p2 = TryApi PATCH "$api/recipes/$rid/publish" $H $null
Check "Publish thanh cong khi da co buoc va nguyen lieu" $p2.ok "status $($p2.status) $($p2.body)"
Check "Trang thai chuyen sang Published" ($p2.data.data.status -eq "Published") "$($p2.data.data.status)"
$publishedAt = $p2.data.data.publishedAt
Check "Co moc thoi gian publishedAt" ($null -ne $publishedAt) "$publishedAt"

# Idempotent: publish lan hai khong duoc bao loi.
$p3 = TryApi PATCH "$api/recipes/$rid/publish" $H $null
Check "Publish lan hai van tra 200 (idempotent)" $p3.ok "status $($p3.status)"

# Unpublish roi publish lai thi publishedAt PHAI giu nguyen moc cu.
$null = TryApi PATCH "$api/recipes/$rid/unpublish" $H $null
$p4 = TryApi PATCH "$api/recipes/$rid/publish" $H $null
Check "Publish lai sau unpublish van giu moc publishedAt cu" ($p4.data.data.publishedAt -eq $publishedAt) "cu=$publishedAt moi=$($p4.data.data.publishedAt)"

$ar = TryApi PATCH "$api/recipes/$rid/archive" $H $null
Check "Archive duoc" ($ar.data.data.status -eq "Archived") "$($ar.data.data.status)"

$null = TryApi PATCH "$api/recipes/$rid/publish" $H $null

# =============================================================================
Muc "8. FR-RCP-008 - anh"

if ($PSVersionTable.PSVersion.Major -ge 7) {
    # Tao mot file PNG 1x1 hop le de upload.
    $png = [Convert]::FromBase64String("iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mP8z8BQDwAEhQGAhKmMIQAAAABJRU5ErkJggg==")
    $tmp = Join-Path $env:TEMP "test-anh-$suffix.png"
    [IO.File]::WriteAllBytes($tmp, $png)

    try {
        $img = Invoke-RestMethod -Method Post -Uri "$api/recipes/$rid/images" -Headers $H `
                                 -Form @{ file = Get-Item $tmp }
        Check "Upload duoc anh" $true ""
        $imgId = $img.data.id
        Check "Anh dau tien tu dong thanh anh dai dien" ($img.data.isPrimary -eq $true) "isPrimary = $($img.data.isPrimary)"

        $iu = TryApi PUT "$api/recipes/$rid/images/$imgId" $H (Json @{ altText = "Bat pho bo" })
        Check "Sua duoc altText cua anh" ($iu.data.data.altText -eq "Bat pho bo") "$($iu.data.data.altText)"

        $idel = TryApi DELETE "$api/recipes/$rid/images/$imgId" $H $null
        Check "Xoa duoc anh" $idel.ok "status $($idel.status)"
    } catch {
        Check "Upload duoc anh" $false "$($_.ErrorDetails.Message)"
    } finally {
        Remove-Item $tmp -ErrorAction SilentlyContinue
    }
} else {
    Write-Host "  [BO QUA] Phan upload anh can PowerShell 7 tro len." -ForegroundColor Yellow
    Write-Host "           Ban dang dung PowerShell $($PSVersionTable.PSVersion). Test thu tay o /scalar." -ForegroundColor DarkGray
}

# =============================================================================
Muc "9. Phan quyen - nguoi khac khong duoc sua cong thuc cua minh"

$suffix2 = Get-Random -Maximum 999999
$reg2 = TryApi POST "$api/auth/register" $null (Json @{
    fullName = "Nguoi La"
    email    = "khach$suffix2@test.local"
    userName = "khach$suffix2"
    password = "Test@1234"
})
if ($reg2.ok) {
    $H2 = @{ Authorization = "Bearer $($reg2.data.data.accessToken)" }
    $x = TryApi PATCH "$api/recipes/$rid/archive" $H2 $null
    Check "Nguoi khac archive cong thuc cua minh bi tu choi 403" ($x.status -eq 403) "status $($x.status)"

    $y = TryApi POST "$api/recipes/$rid/steps" $H2 (Json @{ title = "Pha hoai"; description = "abc" })
    Check "Nguoi khac them buoc vao cong thuc cua minh bi tu choi 403" ($y.status -eq 403) "status $($y.status)"
}

# =============================================================================
Write-Host ""
Write-Host "=============================================" -ForegroundColor White
Write-Host " KET QUA: $script:pass dat / $script:fail hong" -ForegroundColor White
Write-Host "=============================================" -ForegroundColor White
if ($script:fail -eq 0) {
    Write-Host " Ca 3 tinh nang chay dung." -ForegroundColor Green
} else {
    Write-Host " Co muc hong o tren, doc dong [FAIL] de biet cho nao." -ForegroundColor Red
}
Write-Host ""
Write-Host "Cong thuc vua tao: $api/recipes/$slug"
