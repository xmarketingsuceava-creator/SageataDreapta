# Genereaza icon.ico (cerc verde cu sageata alba) - folosit de build.bat
Add-Type -AssemblyName System.Drawing
$sizes = 16, 24, 32, 48, 64, 256
$pngs = @()
foreach ($s in $sizes) {
    $bmp = New-Object Drawing.Bitmap $s, $s
    $g = [Drawing.Graphics]::FromImage($bmp)
    $g.SmoothingMode = 'AntiAlias'
    $g.Clear([Drawing.Color]::Transparent)
    $pad = [Math]::Max(1, $s * 0.03)
    $g.FillEllipse((New-Object Drawing.SolidBrush ([Drawing.Color]::FromArgb(22, 163, 74))), $pad, $pad, $s - 2 * $pad, $s - 2 * $pad)
    $u = $s / 100.0
    $pts = @(
        (New-Object Drawing.PointF (24 * $u), (40 * $u)),
        (New-Object Drawing.PointF (52 * $u), (40 * $u)),
        (New-Object Drawing.PointF (52 * $u), (24 * $u)),
        (New-Object Drawing.PointF (80 * $u), (50 * $u)),
        (New-Object Drawing.PointF (52 * $u), (76 * $u)),
        (New-Object Drawing.PointF (52 * $u), (60 * $u)),
        (New-Object Drawing.PointF (24 * $u), (60 * $u))
    )
    $g.FillPolygon([Drawing.Brushes]::White, [Drawing.PointF[]]$pts)
    $g.Dispose()
    $ms = New-Object IO.MemoryStream
    $bmp.Save($ms, [Drawing.Imaging.ImageFormat]::Png)
    $bmp.Dispose()
    $pngs += , $ms.ToArray()
}
$out = New-Object IO.MemoryStream
$w = New-Object IO.BinaryWriter $out
$w.Write([UInt16]0); $w.Write([UInt16]1); $w.Write([UInt16]$sizes.Count)
$offset = 6 + 16 * $sizes.Count
for ($i = 0; $i -lt $sizes.Count; $i++) {
    $s = $sizes[$i]; $b = if ($s -ge 256) { 0 } else { $s }
    $w.Write([byte]$b); $w.Write([byte]$b); $w.Write([byte]0); $w.Write([byte]0)
    $w.Write([UInt16]1); $w.Write([UInt16]32)
    $w.Write([UInt32]$pngs[$i].Length); $w.Write([UInt32]$offset)
    $offset += $pngs[$i].Length
}
foreach ($p in $pngs) { $w.Write($p) }
$w.Flush()
[IO.File]::WriteAllBytes((Join-Path $PSScriptRoot 'icon.ico'), $out.ToArray())
