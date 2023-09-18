namespace com.etsoo.ImageUtils
{
    /// <summary>
    /// Image shared utilities
    /// 共享的图片工具
    /// </summary>
    public static class ImageShared
    {
        /// <summary>
        /// Shared Rectangle
        /// 共享矩形
        /// </summary>
        public struct Rectangle
        {
            public int X;
            public int Y;
            public int Width;
            public int Height;

            public Rectangle(int x, int y, int width, int height)
            {
                X = x;
                Y = y;
                Width = width;
                Height = height;
            }
        }

        /// <summary>
        /// Calculate source and target size to rectangles
        /// 将源和目标大小计算为矩形
        /// </summary>
        /// <param name="sourceWidth">Source width</param>
        /// <param name="sourceHeight">Source height</param>
        /// <param name="targetWidth">Target width</param>
        /// <param name="targetHeiht">Target height</param>
        /// <param name="cropSource">Crop source or not</param>
        /// <returns>Result</returns>
        public static (Rectangle source, Rectangle target, bool isResizing) Calculate(int sourceWidth, int sourceHeight, int targetWidth, int targetHeiht, bool cropSource = false)
        {
            // Ratio
            var rw = (float)sourceWidth / targetWidth;
            var rh = (float)sourceHeight / targetHeiht;

            int sx = 0, sy = 0;
            int tx = 0, ty = 0;

            if (cropSource)
            {
                // Crop content in center
                if (rw > rh)
                {
                    var newSW = Convert.ToInt32(targetWidth * rh);
                    sx = (sourceWidth - newSW) / 2;
                    sourceWidth = newSW;
                }
                else
                {
                    var newSH = Convert.ToInt32(targetHeiht * rw);
                    sy = (sourceHeight - newSH) / 2;
                    sourceHeight = newSH;
                }

                // Avoid zoom out
                if (targetWidth > sourceWidth)
                    targetWidth = sourceWidth;
                if (targetHeiht > sourceHeight)
                    targetHeiht = sourceHeight;
            }
            else
            {
                // All content in center with target width or height
                if (rw > rh)
                {
                    // Reduce target height
                    var newTH = Convert.ToInt32(sourceHeight / rw);
                    ty = (targetHeiht - newTH) / 2;
                    targetHeiht = newTH;
                }
                else
                {
                    // Reduce target width
                    var newTW = Convert.ToInt32(sourceWidth / rh);
                    tx = (targetWidth - newTW) / 2;
                    targetWidth = newTW;
                }
            }

            var source = new Rectangle(sx, sy, sourceWidth, sourceHeight);
            var target = new Rectangle(tx, ty, targetWidth, targetHeiht);

            return (source, target, rw.Equals(rh));
        }
    }
}
