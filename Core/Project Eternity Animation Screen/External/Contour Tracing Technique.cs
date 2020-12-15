using System.Collections.Generic;

namespace ProjectEternity.GameScreens.AnimationScreen
{
    public class ContourTracingTechnique
    {
        //http://www.codeproject.com/script/Articles/ViewDownloads.aspx?aid=407172
        //Kudos to the author.

        public class CCL_Map2D
        {
            public ushort width;                   // CCL map width
            public ushort height;                  // CCL map height
            public byte[][] map;                     // CCL map data
        }

        public enum CONTOUR_DIR
        {
            CW_DIR = 0,                            // Clockwise contour direction
            CCW_DIR = 1,                            // Counter clockwise contour direction
            UNKNOWN_DIR = 2                         // Unknown contour direction
        }

        public class Vertex2D
        {
            public ushort v_flags;                 // Vertex flags
            public long x;                          // Vertex x co-ord
            public long y;                          // Vertex y co-ord
            public Vertex2D next;                  // Next vertex
            public Vertex2D prev;                  // Prev vertex
        }

        public class Contour2D
        {
            public CONTOUR_DIR Direction;             // Direction of contour
            public ushort Index;                   // Label index of contour
            public Vertex2D FirstVertex;                  // First vertex on this contour
            public Contour2D inside_contours;      // Inside paired contours
            public Contour2D nextpeer;             // Next peer contour
            public Contour2D prevpeer;             // Prev peer contour
        }

        private CCL_Map2D CreateCCLMap2D(ushort Width, ushort Height,  // CCL map width, height to create
                                  byte InitLabel)                           // Label to preset into map
        {
            bool fail;
            ushort y;
            CCL_Map2D p;

            p = new CCL_Map2D();
            if (p == null)
                return (null);                                          // Allocate failed so exit and return NULL
            fail = false;                                                   // Preset fail to false
            p.width = Width;                                                // Hold map width
            p.height = Height;                                              // Hold map height
            p.map = new byte[Height][];                      // Allocate map height of pointers
            if (p.map == null)
                fail = true;                                    // Allocate failed so set fail flag
            y = 0;                                                          // Preset y to zero
            while ((fail == false) && (y < Height))
            {                     // For each map line and we havent failed
                p.map[y] = new byte[Width];    // Allocate map space for this line
                y++;                                                        // Increment y
            }
            if (fail == true)
            {                                              // Something in allocate failed
                return (null);                                                  // Now fail out
            }
            return (p);                                                     // return the CCL_MAP2D created
        }

        private short[][] CreateLabelMap(ushort Width, ushort Height)// Label map width, height to create
        {
            bool fail;
            ushort y;
            long lw;
            short[][] labelmap;

            fail = false;                                                   // Preset fail to FALSE
            labelmap = new short[Height][];           // Label map allocated
            if (labelmap == null)
                fail = true;                                 // Allocate failed so set fail flag
            lw = Width * sizeof(short);                                     // Memory space size of a label map line
            y = 0;                                                          // Preset y to zero
            while ((fail == false) && (y < Height))
            {                     // For each labelmap line and we havent failed
                labelmap[y] = new short[Width];                          // Allocate label space
                if (labelmap[y] != null)
                {                                      // If allocate worked
                }
                else
                    fail = true;                                         // Otherwise set fail flag
                y++;                                                        // Increment y
            }
            if (fail == true)
            {                                              // Something in allocate failed
                return (null);                                                  // Now fail out
            }
            return labelmap;                                              // return the labelmap created
        }

        private static Vertex2D CreateVertex2D(long x, long y,             // Vertex (x,y)
                                  ushort flags)                     // Flags to set for vertex
        {
            Vertex2D P;

            P = new Vertex2D();                       // Allocate memory for vertex
            if (P != null)
            {                                                    // Check allocate does not fail
                P.v_flags = flags;                                          // Set the flags
                P.x = x;                                                    // Set x value
                P.y = y;                                                    // Set y value
            }
            return (P);                                                     // Return the vertex
        }

        private static void DisposeVertexList2D(Vertex2D firstvertex)                    // Pointer to vertex circular list to dispose
        {
            Vertex2D P, Q;

            P = firstvertex;                                                // Start on the first vertex
            while (P != null)
            {                                                 // While P non nil loop
                Q = P.next;                                             // Hold the next vertex ptr
                P = Q;                                                      // Set current vertex to next vertex
                if (P == firstvertex)
                    P = null;                                // We have come to end of loop
            }
        }

        private static void DisposeContourList2D(Contour2D contour)                      // Pointer to contour list to dispose
        {
            Contour2D P, Q;

            P = contour;                                                    // Transfer contour pointer
            while (P != null)
            {
                DisposeVertexList2D(P.FirstVertex);                     // Dispose of the vertex list
                if (P.inside_contours != null)
                    DisposeContourList2D(P.inside_contours);                // Dispose any inside contours
                Q = P.nextpeer;                                         // Hold our next peer
                P = Q;                                                      // Set contour to our next peer
                if (P == contour)
                    P = null;                                    // Complete loop done
            }
        }

        private static short OUTSIDE_EDGE = short.MinValue;
        private static short INSIDE_EDGE = short.MinValue + 1;

        private static short[,] SearchDirection = new short[8, 2] { { 0, 1 }, { 1, 1 }, { 1, 0 }, { 1, -1 }, { 0, -1 }, { -1, -1 }, { -1, 0 }, { -1, 1 } };

        private static bool Tracer(ref long cy, ref long cx,                // Current x,y vertex pointers to update
                           ref ushort dir,                             // Current direction pointer to update
                           byte colour,                                 // Colour we are connecting
                           byte[][] bitmap,                                 // Image data we are running CCL on
                           short[][] labelmap,                                // Label map which is an array of shorts
                           short label,                                     // Label for this CCL
                           short edgelabel)                                 // Edge label if we find edge
        {
            ushort i;
            long y, x;

            for (i = 0; i < 7; i++)
            {                                     // For each of the 8 directions
                y = cy + SearchDirection[dir, 0];                       // Load new y position
                x = cx + SearchDirection[dir, 1];                       // Load new x position
                if ((bitmap[y][x] == colour) &&                             // If pixel matches colour
                    ((labelmap[y][x] == 0) || (labelmap[y][x] == label)))
                {  // And edge is unlabelled or same label
                    labelmap[y][x] = label;                             // Label this point with given label
                    cy = y;                                          // Move to new y value
                    cx = x;                                          // Move to new x value
                    return true;                                        // This pixel is joined to contour
                }
                else
                {
                    if (labelmap[y][x] == 0) labelmap[y][x] = edgelabel;    // Mark this pixel with edge label
                    dir = (ushort)((dir + 1) % 8);                              // Set next tracing direction
                }
            }
            return false;                                                   // None of eight neighbours joined
        }

        private static Vertex2D ContourTracing(long sy, long sx,     // Start x,y vertex
                                        short label,                        // Label value
                                        byte colour,                    // Color to match for CCL
                                        byte[][] bitmap,                    // Image data we are running CCL on
                                        short[][] labelmap,                   // Label map which is an array of shorts
                                        short edgelabel)                    // Edge label if we find edge
        {
            Vertex2D Fv, Cv, P;
            long cx, cy;
            ushort dir, ldir;

            cx = sx;                                                        // Transfer start x position
            cy = sy;                                                        // Transfer start y position
            Fv = null;                                                         // Preset nil return
            dir = 0;                                                        // Preset trace direction to 0
            if (Tracer(ref cy, ref cx, ref dir, colour, bitmap, labelmap, label, edgelabel) == true)
            {// See if we have a possible start point
                Fv = CreateVertex2D(sx, sy, dir);                           // Allocate space for 1st vertex
                if (Fv == null)
                    return (null);                                    // Allocate failed exit and return NULL
                Fv.next = null;                                                // Zero the next vertex ptr
                Fv.prev = null;                                                // Zero the previous vertex ptr
                Cv = Fv;                                                    // Current vertex = this first vertex
                ldir = dir;                                                 // Hold the last tracing direction
                P = CreateVertex2D(cx, cy, 0);                              // Allocate space for this 2nd vertex
                if (P == null)
                {                                                // Allocate failed cleanup and exit with NULL
                    return (null);                                              // Exit and return NULL
                }
                P.prev = Cv;                                                // Prev vertex is the current vertex
                Cv.next = P;                                                // Current vertex next ptr is this vertex
                P.next = null;                                             // Zero the next vertex ptr
                Cv = P;                                                     // Make this vertex the current vertex
                do
                {
                    dir = (ushort)((dir + 6) % 8);                                    // Set new tracing direction to 90 degree of last
                    Tracer(ref cy, ref cx, ref dir, colour, bitmap, labelmap, label, edgelabel);// Trace the next point
                    if (ldir != dir)
                    {                                       // New tracing direction does not match last
                        Cv.v_flags = dir;                                   // Hold direction
                        P = CreateVertex2D(cx, cy, 0);                      // Allocate new vertex space
                        if (P != null)
                        {
                            P.prev = Cv;                                    // This vertex prev is current vertex
                            Cv.next = P;                                    // Current vertex next is this vertex
                            P.next = null;                                 // This vertex next is nil
                            Cv = P;                                         // Set vertex now as current vertex
                        }
                    }
                    else
                    {                                                // Tracing direction same as last point
                        Cv.v_flags = ldir;
                        Cv.x = cx;                                          // Simply move the current vertex x as straight line
                        Cv.y = cy;                                          // Simply move the current vertex y as straight line
                    }
                    ldir = dir;                                             // Hold the last tracing direction
                } while ((cx != sx) || (cy != sy));                         // Terminate if cx==sx && cy==sy
                Fv.prev = Cv.prev;                                      // First vectors previous is currents prev
                Cv.prev.next = Fv;                                      // Current vectors prev next needs to be first
            }
            return Fv;                                                      // Return the contour via first vertex
        }

        public Contour2D Ras2Vec2D(CCL_Map2D imagedata, ushort depthlabel, ref short connectedcomponentscount)
        {
            byte colour, bc;
            short lc, labelindex;
            long cx, cy;
            short[][] labelmap;
            byte[] colormap;
            Vertex2D vertex;
            Contour2D p, cc, sc;

            if (imagedata == null)
                return null;// Check for invalid pointers

            connectedcomponentscount = 0;                                // Zero connected component count
            labelmap = CreateLabelMap(imagedata.width, imagedata.height);   // Create a temporary label map
            if (labelmap == null)
                return (null);                                  // label map fail so exit
            colormap = new byte[short.MaxValue];                      // color map
            if (colormap == null)
            {                                             // Color map allocate failed
                return (null);                                                  // Exit returning failure
            }
            colormap[0] = imagedata.map[0][0];                              // Background colour set
            cc = null;                                                         // Set current contour to null
            for (cy = 1; cy < imagedata.height - 1; cy++)
            {              // For each line in image (except top and bottom most)
                labelindex = 0;                                             // Label index is undefined
                bc = colormap[0];                                           // Reset to global background colour
                for (cx = 1; cx < imagedata.width - 1; cx++)
                {               // For each pixel in this line (except 1st and last)
                    if (labelmap[cy][cx] == 0)
                    {                             // This pixel is unprocessed
                        colour = imagedata.map[cy][cx];                 // Fetch pixel colour at cx,cy
                        if (colour != bc)
                        {                                  // If pixel doesnt match current back color
                            if (labelindex == 0)
                            {                           // We have no label index set yet
                                lc = (short)(connectedcomponentscount + 1);         // If we have a contour this will be its label
                                colormap[lc] = colour;                      // Hold colormap entry
                                bc = colour;                                // Back colour is now this colour
                                vertex = ContourTracing(cy, cx, lc,
                                    colour, imagedata.map, labelmap,
                                    OUTSIDE_EDGE);                          // External contour = clockwise (BLUE)
                                if (vertex != null)
                                {                           // We have a valid vertex from tracing
                                    connectedcomponentscount++;          // Inc connect component count
                                    labelindex = lc;                        // Set new labelindex
                                    p = new Contour2D();                    // Allocate memory
                                    if (p != null)
                                    {
                                        p.Direction = CONTOUR_DIR.CW_DIR;               // Contour is clockwise (blue)
                                        p.Index = (ushort)labelindex;               // Temp use depth label to hold index
                                        p.FirstVertex = vertex;         // Hold first vertex point
                                        p.nextpeer = cc;                    // Current contour is our next peer
                                        p.prevpeer = null;                 // We have no previous contour
                                        p.inside_contours = null;              // Initially we have no inside contours
                                        if (cc != null)
                                            cc.prevpeer = p;       // If current contour valid we are it's prev peer
                                        cc = p;                             // We are now the current contour
                                    }
                                }
                            }
                            else
                            {
                                vertex = ContourTracing(cy, cx, INSIDE_EDGE,
                                    colour, imagedata.map, labelmap,
                                    labelindex);                            // Internal contour anti-clockwise (RED)
                                if (vertex != null)
                                {                           // We have a valid vertex from tracing
                                    p = new Contour2D();// Allocate memory
                                    if (p != null)
                                    {
                                        p.FirstVertex = vertex;         // Hold first vertex
                                        p.inside_contours = null;              // Initially we have no inside contours
                                        if (depthlabel == 0)
                                            p.Direction = CONTOUR_DIR.CCW_DIR;// Contour is anti-clockwise (red)
                                        else
                                            p.Direction = CONTOUR_DIR.CW_DIR;      // Contour is clockwise (blue)
                                        p.Index = depthlabel;               // Contour is inside so safe to label depth
                                        // All red contours belong to a paired blue find it
                                        sc = cc;                            // Start on current contour
                                        while ((sc != null) && (sc.Index != labelindex))
                                            sc = sc.nextpeer;// Find outer contour pair
                                        if (sc != null)
                                        {                       // Pair found as it always should
                                            p.nextpeer = sc.inside_contours;// Link to current inside pointer
                                            if (sc.inside_contours != null)
                                                sc.inside_contours.prevpeer = p;// Link inside contour to us
                                            sc.inside_contours = p;     // Set us as current inside contour
                                        }
                                        else
                                            DisposeContourList2D(p);     // SHOULD BE IMPOSSIBLE TO GET HERE BUT FOR SAFETY
                                    }
                                }
                            }
                        }
                        else
                            labelmap[cy][cx] = labelindex;               // Matches colour so simply fill in label
                    }
                    else if ((labelmap[cy][cx] == OUTSIDE_EDGE) || (labelmap[cy][cx] == INSIDE_EDGE))
                    {
                        labelindex = 0;                                     // Labelindex undefined
                        if (labelmap[cy][cx] == INSIDE_EDGE) bc = imagedata.map[cy][cx];// Inside edge keep image colour as back
                        else bc = colormap[0];                          // Outside edge use global backcolour
                    }
                    else
                    {
                        labelindex = labelmap[cy][cx];                      // Set the label index
                        bc = colormap[labelindex];                          // Set the background colour to the indexed colour
                    }
                }                                                          // for cx loop end
            }                                                              // for cy loop end
            sc = cc;                                                        // Start on current contour
            while (sc != null)
            {
                sc.Index = depthlabel;                                      // Reclaim the index to record depth
                sc = sc.nextpeer;                                           // Move to next contour
            }
            return cc;                                                      // Return current contour
        }

        public static void DisplayContour(Core.CustomSpriteBatch g, Microsoft.Xna.Framework.Graphics.Texture2D sprPixel,
                             Contour2D FirstContour)                       // Pointer to first contour
        {
            Vertex2D P;
            Contour2D Q, PQ, IQ;

            Microsoft.Xna.Framework.Color DrawColor = Microsoft.Xna.Framework.Color.White;

            PQ = FirstContour;                                              // Start on first contour
            if (PQ != null)
                IQ = PQ.inside_contours;
            else
                IQ = null;         // Fetch any inside contour
            while (PQ != null)
            {                                                // While process contour valid
                if (IQ != null)
                {                                              // Check for inside contour
                    Q = IQ;                                                 // Set Q to inside contour
                    IQ = IQ.nextpeer;                                       // Set IQ to next peer contour
                }
                else
                {                                                    // We have no inside contour
                    Q = PQ;                                                 // Set Q to this contour
                    PQ = PQ.nextpeer;                                       // Set PQ to next peer contour
                    if (PQ != null)
                        IQ = PQ.inside_contours;
                    else
                        IQ = null; // Fetch any inside contour of that next peer
                }
                switch (Q.Direction)
                {                                       // Contour direction will set colour
                    case CONTOUR_DIR.CW_DIR:                                            // CLOCK WISE CONTOUR
                        DrawColor = Microsoft.Xna.Framework.Color.Blue;             // Blue pen
                        break;

                    case CONTOUR_DIR.CCW_DIR:                                           // COUNTER CLOCKWISE CONTOUR
                        DrawColor = Microsoft.Xna.Framework.Color.Red;             // Red pen
                        continue;//Don't draw it as it's probably not useful
                    case CONTOUR_DIR.UNKNOWN_DIR:                                       // UNKNOWN DIRECTION CONTOUR
                        DrawColor = Microsoft.Xna.Framework.Color.Green;           // Green pen
                        break;
                }

                P = Q.FirstVertex;                                          // Start on first vertex
                Microsoft.Xna.Framework.Vector2 StartPos = new Microsoft.Xna.Framework.Vector2((int)P.x, (int)P.y);
                Microsoft.Xna.Framework.Vector2 EndPos = new Microsoft.Xna.Framework.Vector2();
                do
                {
                    P = P.next;                                         // Advance to next vertex pointer
                    if ((P.v_flags & 0x08) == 0)
                    {                  // Not a lift close move
                        EndPos.X = (int)P.x;
                        EndPos.Y = (int)P.y;
                        DrawLine(g, sprPixel, StartPos, EndPos, DrawColor);
                        StartPos.X = (int)P.x;
                        StartPos.Y = (int)P.y;
                    }
                }
                while (P != Q.FirstVertex);                               // Until we finish the loop
            }
        }

        public static HashSet<Shape> GetContour(Contour2D FirstContour)                       // Pointer to first contour
        {
            HashSet<Shape> HashShape = new HashSet<Shape>();

            Vertex2D P;
            Contour2D Q, PQ, IQ;
            Shape NewShape;

            PQ = FirstContour;                                              // Start on first contour
            if (PQ != null)
                IQ = PQ.inside_contours;
            else
                IQ = null;         // Fetch any inside contour

            while (PQ != null)
            {                                                // While process contour valid
                if (IQ != null)
                {                                              // Check for inside contour
                    Q = IQ;                                                 // Set Q to inside contour
                    IQ = IQ.nextpeer;                                       // Set IQ to next peer contour
                }
                else
                {                                                    // We have no inside contour
                    Q = PQ;                                                 // Set Q to this contour
                    PQ = PQ.nextpeer;                                       // Set PQ to next peer contour
                    if (PQ != null)
                        IQ = PQ.inside_contours;
                    else
                        IQ = null; // Fetch any inside contour of that next peer
                }
                switch (Q.Direction)
                {                                       // Contour direction will set colour
                    case CONTOUR_DIR.CW_DIR:                                            // CLOCK WISE CONTOUR
                        break;

                    case CONTOUR_DIR.CCW_DIR:                                           // COUNTER CLOCKWISE CONTOUR
                        continue;//Don't draw it as it's probably not useful
                    case CONTOUR_DIR.UNKNOWN_DIR:                                       // UNKNOWN DIRECTION CONTOUR
                        break;
                }

                P = Q.FirstVertex;                                          // Start on first vertex
                Microsoft.Xna.Framework.Vector2 StartPos = new Microsoft.Xna.Framework.Vector2((int)P.x, (int)P.y);
                Microsoft.Xna.Framework.Vector2 EndPos = new Microsoft.Xna.Framework.Vector2();

                NewShape = new Shape(StartPos);
                do
                {
                    P = P.next;                                         // Advance to next vertex pointer
                    if ((P.v_flags & 0x08) == 0)// Not a lift close move
                    {
                        EndPos.X = (int)P.x;
                        EndPos.Y = (int)P.y;
                        NewShape.Add(EndPos);
                    }
                }
                while (P != Q.FirstVertex);                               // Until we finish the loop

                --NewShape.X1;
                --NewShape.Y1;
                HashShape.Add(NewShape);
            }
            return HashShape;
        }

        private static void DrawLine(Core.CustomSpriteBatch g, Microsoft.Xna.Framework.Graphics.Texture2D sprPixel, Microsoft.Xna.Framework.Vector2 StartPos, Microsoft.Xna.Framework.Vector2 EndPos, Microsoft.Xna.Framework.Color DrawColor)
        {
            Microsoft.Xna.Framework.Vector2 ScaleFactor = new Microsoft.Xna.Framework.Vector2(EndPos.X - StartPos.X, EndPos.Y - StartPos.Y);
            ScaleFactor.Normalize();
            while ((int)StartPos.X != EndPos.X)
            {
                g.Draw(sprPixel, StartPos, DrawColor);
                StartPos += ScaleFactor;
            }
        }

        public void load_bmp(Microsoft.Xna.Framework.Graphics.Texture2D InputTexture, ref CCL_Map2D FirstImage)
        {
            byte i;
            int x, y;
            Colorstruct Col;

            if (InputTexture == null)
                return;                                       // Bitmap laod failed so exit

            if ((InputTexture.Width > 32765) || (InputTexture.Height > 32765))
            {               // Bitmap is too big to vectorize (outside int range)
                return;                                                  // Fail return
            }
            FirstImage = CreateCCLMap2D((ushort)(InputTexture.Width + 2),
                (ushort)(InputTexture.Height + 2), 0);                                 // Create a CCL map (IT IS BLANK HERE)

            Colorstruct[] TextureData = new Colorstruct[InputTexture.Width * InputTexture.Height];
            InputTexture.GetData(TextureData);

            for (y = 0; y < InputTexture.Height; y++)
            {                                // For each y height scanline
                for (x = 0; x < InputTexture.Width; x++)
                {                         // For each x pixel
                    Col = TextureData[x + y * InputTexture.Width];                               // Fetch the pixel colour
                    if (Col.A > 10)
                        i = 1;
                    else
                        i = 0;      // Convert to B/W depending colour
                    FirstImage.map[y + 1][x + 1] = i;                           // Set B/W colour to CCL Map
                }
            }
        }
    }
}
