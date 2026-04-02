# setup_env.ps1 - Tự động cấu hình phím bảo mật cho SentinelStream

$envFile = ".env"
$exampleFile = ".env.example"

# 1. Kiểm tra file .env
if (-not (Test-Path $envFile)) {
    Write-Host "Copying $exampleFile to $envFile..." -ForegroundColor Cyan
    Copy-Item $exampleFile $envFile
}

# 2. Hàm tạo chuỗi ngẫu nhiên
function Generate-RandomHex($length) {
    $bytes = New-Object Byte[] $length
    [System.Security.Cryptography.RandomNumberGenerator]::Create().GetBytes($bytes)
    return [System.BitConverter]::ToString($bytes).Replace("-", "").ToLower()
}

function Generate-RandomSalt($length) {
    $charSet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*"
    $random = New-Object System.Random
    $salt = ""
    for ($i = 0; $i -lt $length; $i++) {
        $salt += $charSet[$random.Next(0, $charSet.Length)]
    }
    return $salt
}

# 3. Tạo Key mới
$newKey = Generate-RandomHex 32  # 32 bytes = 64 characters hex (AES-256)
$newSalt = Generate-RandomSalt 24 # 24 characters random salt

# 4. Cập nhật file .env
$content = Get-Content $envFile
$content = $content -replace "ENCRYPTION_KEY=.*", "ENCRYPTION_KEY=$newKey"
$content = $content -replace "FORENSIC_SALT=.*", "FORENSIC_SALT=$newSalt"
$content | Set-Content $envFile

Write-Host "------------------------------------------------" -ForegroundColor Green
Write-Host "SUCCESS: .env has been updated with secure keys!" -ForegroundColor Green
Write-Host "ENCRYPTION_KEY set to: $newKey"
Write-Host "FORENSIC_SALT set to: $newSalt"
Write-Host "------------------------------------------------" -ForegroundColor Green
Write-Host "Next Step: Fill in your AGORA_APP_ID and AGORA_APP_CERTIFICATE manually." -ForegroundColor Yellow
