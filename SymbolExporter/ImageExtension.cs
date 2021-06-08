using System;

using AppKit;
using Foundation;
using CoreGraphics;

namespace SymbolExporter
{
    public static class ImageExtension
    {
        public static void SaveImageFile(this NSImage sourceImage, string source, nfloat width, nfloat height, NSBitmapImageFileType fileType)
        {
            var image = sourceImage.ResizeSFImage(width, height);
            var imageRep = image.UnscaledBitmapImageRep();
            var imageRep1 = new NSBitmapImageRep(image.AsTiff());
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

            var image = new NSImage(new CGSize(sourceImage.Size.Width * 2, sourceImage.Size.Height * 2));
            image.LockFocus();
            var transform = new NSAffineTransform();
            transform.Scale(2);
            transform.Concat();
            sourceImage.Draw(CGPoint.Empty, CGRect.Empty, NSCompositingOperation.Copy, 1);
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
            image.Draw(CGPoint.Empty, CGRect.Empty, NSCompositingOperation.SourceOver, 1);
            NSGraphicsContext.GlobalRestoreGraphicsState();

            return imageRep;
        }

        public static NSImage ImageRepresentation(this NSView view)
        {
            var size = view.Bounds.Size;
            var imageSize = new CGSize(size.Width, size.Height);

            NSBitmapImageRep bir = view.BitmapImageRepForCachingDisplayInRect(view.Bounds);
            bir.Size = imageSize;
            view.CacheDisplay(view.Bounds, bir);

            var image = new NSImage(imageSize);
            image.AddRepresentation(bir);

            return image;
        }

        public static void SaveViewToPng(this NSView view, string source, NSBitmapImageFileType fileType)
        {
            var image = view.ImageRepresentation();
            //var imageRep = new NSBitmapImageRep(image.AsTiff());
            var imageRep = image.UnscaledBitmapImageRep();
            var data = imageRep.RepresentationUsingTypeProperties(fileType);
            data.Save(source, false);
        }
    }
}
