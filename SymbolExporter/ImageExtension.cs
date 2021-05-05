using System;

using AppKit;
using CoreGraphics;
using Foundation;

namespace SymbolExporter
{
    public static class ImageExtension
    {
        public static void SaveImageFile(this NSImage sourceImage, string source, nfloat width, nfloat height, NSBitmapImageFileType fileType)
        {
            var image = sourceImage.ResizeSFImage(width, height);
            var imageRep = image.UnscaledBitmapImageRep();
            var data = imageRep.RepresentationUsingTypeProperties(fileType);

            data.Save(source, false);
        }

        public static NSImage ResizeSFImage(this NSImage sourceImage, nfloat width, nfloat height)
        {
            var ratio1 = width / sourceImage.Size.Width;
            var ratio2 = height / sourceImage.Size.Height;

            nfloat factor;
            CGPoint point;

            if (ratio1 < ratio2)
            {
                factor = ratio1;
                var margin = (height - sourceImage.Size.Height * factor) / 2 / factor;
                point = new CGPoint(0, margin);
            }
            else
            {
                factor = ratio2;
                var margin = (width - sourceImage.Size.Width * factor) / 2 / factor;
                point = new CGPoint(margin, 0);
            }

            var image = new NSImage(new CGSize(width, height));
            image.LockFocus();
            NSAffineTransform transform = new NSAffineTransform();
            transform.Scale(factor);
            transform.Concat();
            sourceImage.Draw(point, CGRect.Empty, NSCompositingOperation.Copy, 1);
            image.UnlockFocus();
            image.DangerousAutorelease();

            return image;
        }

        public static NSBitmapImageRep UnscaledBitmapImageRep(this NSImage image)
        {
            var imageRep = new NSBitmapImageRep(IntPtr.Zero, (nint)image.Size.Width, (nint)image.Size.Height, 8, 4, true, false, NSColorSpace.DeviceRGB, 0, 0);
            imageRep.Size = image.Size;

            NSGraphicsContext.GlobalSaveGraphicsState();
            NSGraphicsContext.CurrentContext = NSGraphicsContext.FromBitmap(imageRep);
            image.Draw(new CGPoint(0, 0), CGRect.Empty, NSCompositingOperation.SourceOver, 1);
            NSGraphicsContext.GlobalRestoreGraphicsState();

            return imageRep;
        }

    }
}
