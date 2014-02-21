using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using BarcodeLib;

namespace MWBCameraDemo
{
    class BarcodeHelper
    {

        public static Boolean PDF_OPTIMIZED = false;

        public static Windows.Foundation.Rect RECT_LANDSCAPE_1D = new Windows.Foundation.Rect(2, 20, 96, 60);
        public static Windows.Foundation.Rect RECT_LANDSCAPE_2D = new Windows.Foundation.Rect(20, 5, 60, 90);
        public static Windows.Foundation.Rect RECT_PORTRAIT_1D = new Windows.Foundation.Rect(20, 2, 60, 96);
        public static Windows.Foundation.Rect RECT_PORTRAIT_2D = new Windows.Foundation.Rect(20, 5, 60, 90);
        public static Windows.Foundation.Rect RECT_FULL_1D = new Windows.Foundation.Rect(2, 2, 96, 96);
        public static Windows.Foundation.Rect RECT_FULL_2D = new Windows.Foundation.Rect(20, 5, 60, 90);

        public static void initDecoder()
        {

            // register your copy of the mobiScan SDK with the given user name / key
            Scanner.MWBregisterCode(Scanner.MWB_CODE_MASK_25, "username", "key");
            Scanner.MWBregisterCode(Scanner.MWB_CODE_MASK_39, "username", "key");
            Scanner.MWBregisterCode(Scanner.MWB_CODE_MASK_93, "username", "key");
            Scanner.MWBregisterCode(Scanner.MWB_CODE_MASK_128, "username", "key");
            Scanner.MWBregisterCode(Scanner.MWB_CODE_MASK_AZTEC, "username", "key");
            Scanner.MWBregisterCode(Scanner.MWB_CODE_MASK_DM, "username", "key");
            Scanner.MWBregisterCode(Scanner.MWB_CODE_MASK_EANUPC, "username", "key");
            Scanner.MWBregisterCode(Scanner.MWB_CODE_MASK_PDF, "username", "key");
            Scanner.MWBregisterCode(Scanner.MWB_CODE_MASK_QR, "username", "key");
            Scanner.MWBregisterCode(Scanner.MWB_CODE_MASK_RSS, "username", "key");
            Scanner.MWBregisterCode(Scanner.MWB_CODE_MASK_CODABAR, "username", "key");

        
            // choose code type or types you want to search for

            if (PDF_OPTIMIZED)
            {
                Scanner.MWBsetActiveCodes(Scanner.MWB_CODE_MASK_PDF);
                Scanner.MWBsetDirection((uint)(Scanner.MWB_SCANDIRECTION_HORIZONTAL));
                MWBsetScanningRect(Scanner.MWB_CODE_MASK_PDF, RECT_LANDSCAPE_1D);
            }
            else
            {

                // Our sample app is configured by default to search all supported barcodes...
                Scanner.MWBsetActiveCodes(Scanner.MWB_CODE_MASK_25 |
                                          Scanner.MWB_CODE_MASK_39 |
                                          Scanner.MWB_CODE_MASK_93 |
                                          Scanner.MWB_CODE_MASK_128 |
                                          Scanner.MWB_CODE_MASK_AZTEC |
                                          Scanner.MWB_CODE_MASK_DM |
                                          Scanner.MWB_CODE_MASK_EANUPC |
                                          Scanner.MWB_CODE_MASK_PDF |
                                          Scanner.MWB_CODE_MASK_QR |
                                          Scanner.MWB_CODE_MASK_CODABAR |
                                          Scanner.MWB_CODE_MASK_RSS);
                // Our sample app is configured by default to search both directions...
                Scanner.MWBsetDirection((uint)(Scanner.MWB_SCANDIRECTION_HORIZONTAL | Scanner.MWB_SCANDIRECTION_VERTICAL));

                // set the scanning rectangle based on scan direction(format in pct: x, y, width, height)
                MWBsetScanningRect(Scanner.MWB_CODE_MASK_25, RECT_FULL_1D);
                MWBsetScanningRect(Scanner.MWB_CODE_MASK_39, RECT_FULL_1D);
                MWBsetScanningRect(Scanner.MWB_CODE_MASK_93, RECT_FULL_1D);
                MWBsetScanningRect(Scanner.MWB_CODE_MASK_128, RECT_FULL_1D);
                MWBsetScanningRect(Scanner.MWB_CODE_MASK_AZTEC, RECT_FULL_2D);
                MWBsetScanningRect(Scanner.MWB_CODE_MASK_DM, RECT_FULL_2D);
                MWBsetScanningRect(Scanner.MWB_CODE_MASK_EANUPC, RECT_FULL_1D);
                MWBsetScanningRect(Scanner.MWB_CODE_MASK_PDF, RECT_FULL_1D);
                MWBsetScanningRect(Scanner.MWB_CODE_MASK_QR, RECT_FULL_2D);
                MWBsetScanningRect(Scanner.MWB_CODE_MASK_RSS, RECT_FULL_1D);
                MWBsetScanningRect(Scanner.MWB_CODE_MASK_CODABAR, RECT_FULL_1D);

            }

            // But for better performance, only activate the symbologies your application requires...
            // Scanner.MWBsetActiveCodes( Scanner.MWB_CODE_MASK_25 ); 
            // Scanner.MWBsetActiveCodes( Scanner.MWB_CODE_MASK_39 ); 
            // Scanner.MWBsetActiveCodes( Scanner.MWB_CODE_MASK_93 ); 
            // Scanner.MWBsetActiveCodes( Scanner.MWB_CODE_MASK_128 ); 
            // Scanner.MWBsetActiveCodes( Scanner.MWB_CODE_MASK_AZTEC ); 
            // Scanner.MWBsetActiveCodes( Scanner.MWB_CODE_MASK_DM ); 
            // Scanner.MWBsetActiveCodes( Scanner.MWB_CODE_MASK_EANUPC ); 
            // Scanner.MWBsetActiveCodes( Scanner.MWB_CODE_MASK_PDF ); 
            // Scanner.MWBsetActiveCodes( Scanner.MWB_CODE_MASK_QR ); 
            // Scanner.MWBsetActiveCodes( Scanner.MWB_CODE_MASK_RSS ); 
            // Scanner.MWBsetActiveCodes( Scanner.MWB_CODE_MASK_CODABAR ); 
           

            // But for better performance, set like this for PORTRAIT scanning...
            // Scanner.MWBsetDirection((uint)Scanner.MWB_SCANDIRECTION_VERTICAL);
            // set the scanning rectangle based on scan direction(format in pct: x, y, width, height)
            // MWBsetScanningRect(Scanner.MWB_CODE_MASK_25,     RECT_PORTRAIT_1D);     
            // MWBsetScanningRect(Scanner.MWB_CODE_MASK_39,     RECT_PORTRAIT_1D);     
            // MWBsetScanningRect(Scanner.MWB_CODE_MASK_93,     RECT_PORTRAIT_1D); 
            // MWBsetScanningRect(Scanner.MWB_CODE_MASK_128,    RECT_PORTRAIT_1D);
            // MWBsetScanningRect(Scanner.MWB_CODE_MASK_AZTEC,  RECT_PORTRAIT_2D);    
            // MWBsetScanningRect(Scanner.MWB_CODE_MASK_DM,     RECT_PORTRAIT_2D);    
            // MWBsetScanningRect(Scanner.MWB_CODE_MASK_EANUPC, RECT_PORTRAIT_1D);     
            // MWBsetScanningRect(Scanner.MWB_CODE_MASK_PDF,    RECT_PORTRAIT_1D);
            // MWBsetScanningRect(Scanner.MWB_CODE_MASK_QR,     RECT_PORTRAIT_2D);     
            // MWBsetScanningRect(Scanner.MWB_CODE_MASK_RSS,    RECT_PORTRAIT_1D);     
            // MWBsetScanningRect(Scanner.MWB_CODE_MASK_CODABAR,RECT_PORTRAIT_1D); 

            // or like this for LANDSCAPE scanning - Preferred for dense or wide codes...
            // Scanner.MWBsetDirection((uint)Scanner.MWB_SCANDIRECTION_HORIZONTAL);
            // set the scanning rectangle based on scan direction(format in pct: x, y, width, height)
            // MWBsetScanningRect(Scanner.MWB_CODE_MASK_25,     RECT_LANDSCAPE_1D);     
            // MWBsetScanningRect(Scanner.MWB_CODE_MASK_39,     RECT_LANDSCAPE_1D);     
            // MWBsetScanningRect(Scanner.MWB_CODE_MASK_93,     RECT_LANDSCAPE_1D);    
            // MWBsetScanningRect(Scanner.MWB_CODE_MASK_128,    RECT_LANDSCAPE_1D);
            // MWBsetScanningRect(Scanner.MWB_CODE_MASK_AZTEC,  RECT_LANDSCAPE_2D);    
            // MWBsetScanningRect(Scanner.MWB_CODE_MASK_DM,     RECT_LANDSCAPE_2D);    
            // MWBsetScanningRect(Scanner.MWB_CODE_MASK_EANUPC, RECT_LANDSCAPE_1D);     
            // MWBsetScanningRect(Scanner.MWB_CODE_MASK_PDF,    RECT_LANDSCAPE_1D);
            // MWBsetScanningRect(Scanner.MWB_CODE_MASK_QR,     RECT_LANDSCAPE_2D);     
            // MWBsetScanningRect(Scanner.MWB_CODE_MASK_RSS,    RECT_LANDSCAPE_1D); 
            // MWBsetScanningRect(Scanner.MWB_CODE_MASK_CODABAR,RECT_LANDSCAPE_1D); 


            // set decoder effort level (1 - 5)
            // for live scanning scenarios, a setting between 1 to 3 will suffice
            // levels 4 and 5 are typically reserved for batch scanning 
            Scanner.MWBsetLevel(2);

        }

        public static void MWBsetScanningRect(int codeMask, Windows.Foundation.Rect rect)
        {
            Scanner.MWBsetScanningRect(codeMask, (float)rect.Left, (float)rect.Top, (float)rect.Width, (float)rect.Height);
        }

        public static Windows.Foundation.Rect MWBgetScanningRect(int codeMask)
        {
            float left, top, width, height;
            Scanner.MWBgetScanningRect(codeMask, out left, out top, out width, out height);

            return new Windows.Foundation.Rect(left, top, width, height);
        }

        public static String getBarcodeName(int bcType)
        {
            String typeName = "Unknown";
            if (bcType == Scanner.FOUND_128) typeName = "Code 128";
            if (bcType == Scanner.FOUND_39) typeName = "Code 39";
            if (bcType == Scanner.FOUND_DM) typeName = "Datamatrix";
            if (bcType == Scanner.FOUND_EAN_13) typeName = "EAN 13";
            if (bcType == Scanner.FOUND_EAN_8) typeName = "EAN 8";
            if (bcType == Scanner.FOUND_NONE) typeName = "None";
            if (bcType == Scanner.FOUND_RSS_14) typeName = "Databar 14";
            if (bcType == Scanner.FOUND_RSS_14_STACK) typeName = "Databar 14 Stacked";
            if (bcType == Scanner.FOUND_RSS_EXP) typeName = "Databar Expanded";
            if (bcType == Scanner.FOUND_RSS_LIM) typeName = "Databar Limited";
            if (bcType == Scanner.FOUND_UPC_A) typeName = "UPC A";
            if (bcType == Scanner.FOUND_UPC_E) typeName = "UPC E";
            if (bcType == Scanner.FOUND_PDF) typeName = "PDF417";
            if (bcType == Scanner.FOUND_QR) typeName = "QR";
            if (bcType == Scanner.FOUND_AZTEC) typeName = "Aztec";
            if (bcType == Scanner.FOUND_25_INTERLEAVED) typeName = "Code 25 Interleaved";
            if (bcType == Scanner.FOUND_25_STANDARD) typeName = "Code 25 Standard";
            if (bcType == Scanner.FOUND_93) typeName = "Code 93";
            if (bcType == Scanner.FOUND_CODABAR) typeName = "Codabar";

            return typeName;
        }

        static public Byte[] BufferFromImage(BitmapImage imageSource)
        {
            WriteableBitmap wb = new WriteableBitmap(imageSource);

            int px = wb.PixelWidth;
            int py = wb.PixelHeight;

            Byte[] res = new Byte[px * py];

            for (int y = 0; y < py; y++)
            {
                for (int x = 0; x < px; x++)
                {
                    int color = wb.Pixels[y * px + x];
                    res[y * px + x] = (byte)color;
                }
            }

            return res;


        }


    }
}
