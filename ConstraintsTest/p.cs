using System;
using Inventor;
using System.Collections.Generic;
using Figgle;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.IO;
using System.IO.Ports;
using System.Security.Policy;
using System.Windows.Forms;
using System.Web;
using System.Data.SQLite;

// 9/5/2023: Worked on checking to see if there were interferances between flanges and the cross; Also worked on creating a lookup table for standard tubing sizes and their combatibility with the flanges
// 9/14/2023: Finished lookup table for standard tubing sizes; Added more flanges to collection that covered the entirety of the tubing sizes available. Fixed issue where CF flanges were overlapping
// 9/21/2023: Added a function to remove the colors from the cross part and be metal 
// 11/14/2023: Added and began working on integration of SQL database to store flange information and access this information
// 11/21/2023: Finished integration of SQL database to store flange information and access this information. Started working on fixing methods to match new database
namespace InventorTest
{
    class Program
    {
       
        // Access the active assembly document and set up reference items
        static Inventor.Application inventorApp = System.Runtime.InteropServices.Marshal.GetActiveObject("Inventor.Application") as Inventor.Application;

        // Get the active assembly document
        static AssemblyDocument assemblyDoc = inventorApp.ActiveDocument as AssemblyDocument;
        
        static UnitsOfMeasure units = assemblyDoc.UnitsOfMeasure;
        
        
        static string crossFile = "C:\\Users\\Liam\\Desktop\\Parts\\4cross.ipt";
        static Dictionary<int, string> colorCodes = new Dictionary<int, string>()
        {
            {1, "Yellow" },
            {2, "Orange" },
            {3, "Black" },
            {4, "Red" },
            {5, "Cyan" },
            {6, "Gold" }
        };
        static Dictionary<string, Diameters> partLookup = generateLookupTable("C:\\Users\\Liam\\Desktop\\Parts\\FaceAreas.txt");
        static Dictionary<string, Diameters> tubingLookup = generateTubingLookupTable("C:\\Users\\Liam\\Desktop\\Parts\\StandardPipeSizes.txt");
        static void Main(string[] args)
        {
            //Console.WriteLine(units.LengthUnits);
            //UpdatePipeDiameters("N_ID", "N_OD", 2, 2.1);
            //return;
            //Dictionary<string, Diameters> tubingLookup = generateTubingLookupTable("C:\\Users\\Liam\\Desktop\\Parts\\StandardPipeSizes.txt");

            //printColorCode(colorCodes);

            //foreach (KeyValuePair<string, Diameters> ele2 in partLookup)
            //{
            //    Console.WriteLine("Part: {0}; ID: {1}, OD: {2}", ele2.Key, ele2.Value.InnerDiameter, ele2.Value.OuterDiameter);
            //}

            //Console.WriteLine($"{GetClosestNotLarger(1.6, tubingLookup)} Poopy");

            // Create array of flanges that can be added to the assembly
            // Dictionary <int, FlangeIdentifier> selectedFlanges = new Dictionary<int, FlangeIdentifier>();
            FlangeIdentifier[] selectedFlanges = new FlangeIdentifier[6];

            bool exit = false;

            while(!exit)
            {
                FlangeIdentifier oF = null;
                SQL.printMainMenu(selectedFlanges);
                var choice = Console.ReadLine();
                switch(choice)
                {
                    case "1":
                        oF = SQL.flangeMenu("1");
                        //Console.WriteLine(oF.Part.ToString());
                        selectedFlanges[0] = oF;
                        break;
                    case "2":
                        oF = SQL.flangeMenu("2");
                        //Console.WriteLine(oF.Part.ToString());
                        selectedFlanges[1] = oF;
                        break;
                    case "3":
                        oF = SQL.flangeMenu("3");
                        //Console.WriteLine(oF.Part.ToString());
                        selectedFlanges[2] = oF;
                        break;
                    case "4":
                        oF = SQL.flangeMenu("4");
                        //Console.WriteLine(oF.Part.ToString());
                        selectedFlanges[3] = oF;
                        break;
                    case "5":
                        oF = SQL.flangeMenu("5");
                        //Console.WriteLine(oF.Part.ToString());
                        selectedFlanges[4] = oF;
                        break;
                    case "6":
                        oF = SQL.flangeMenu("6");
                        //Console.WriteLine(oF.Part.ToString());
                        selectedFlanges[5] = oF;
                        break;
                    case "7":
                        exit = true;
                        break;
                }
                    

            }
            





            
            /*
             if (changeColorToGrey())
            {
                return;
            }
            if (restoreToDefaults())
            {
                return;
            }
            string flange1File = $"C:\\Users\\Liam\\Desktop\\Parts\\{selectedFlanges[0].Part}.ipt";
            string flange2File = $"C:\\Users\\Liam\\Desktop\\Parts\\{selectedFlanges[1].Part}.ipt";
            string flange3File = $"C:\\Users\\Liam\\Desktop\\Parts\\{selectedFlanges[2].Part}.ipt";
            string flange4File = $"C:\\Users\\Liam\\Desktop\\Parts\\{selectedFlanges[3].Part}.ipt";
            string flange5File = $"C:\\Users\\Liam\\Desktop\\Parts\\{selectedFlanges[4].Part}.ipt";
            string flange6File = $"C:\\Users\\Liam\\Desktop\\Parts\\{selectedFlanges[5].Part}.ipt";


             */
            
           

            
           // return;
            // Check if the assembly document is active, if not throw an exception
            if (assemblyDoc == null)
            {
                throw new InvalidOperationException("No assembly document is active.");
            }
            assemblyDoc.Update();

            // Create an assembly component definition
            AssemblyComponentDefinition assemblyComponentDefinition = assemblyDoc.ComponentDefinition;

            // Inialize an object collection to add parts to
            ObjectCollection flangeCollection = inventorApp.TransientObjects.CreateObjectCollection();

            ComponentOccurrence cross = AddPartToAssembly(crossFile, assemblyDoc);
            
            
            //return;
            //UpdatePipeLength();
            //UpdatePipeLength();
            //return;

            /*
             ComponentOccurrence flange1_1 = AddPartToAssembly(flange1File, assemblyDoc);
            ComponentOccurrence flange1_2 = AddPartToAssembly(flange2File, assemblyDoc);
            ComponentOccurrence flange2_1 = AddPartToAssembly(flange3File, assemblyDoc);
            ComponentOccurrence flange2_2 = AddPartToAssembly(flange4File, assemblyDoc);
            ComponentOccurrence flange3_1 = AddPartToAssembly(flange5File, assemblyDoc);
            ComponentOccurrence flange3_2 = AddPartToAssembly(flange6File, assemblyDoc);
             */
            

            flangeCollection.Add(cross);

            // Eventually will be replaced by a asynchronous user input function that will add flanges to the assembly
            for (int i = 0; i < selectedFlanges.Length; i++)
            {
                // Generate file path
                string flangeFile = $"C:\\Users\\Liam\\Desktop\\Parts\\allsizes\\{(selectedFlanges[i].Part).ToString()}.ipt";
                Console.WriteLine(flangeFile);

                // Add part to assembly
                ComponentOccurrence flange = AddPartToAssembly(flangeFile, assemblyDoc);

                // Add constraints between parts
                AddConstraintsBetweenParts(cross, flange, selectedFlanges[i].Part, selectedFlanges[i], flangeCollection);
            }

            /*
             AddConstraintsBetweenParts(cross, flange1_1, selectedFlanges[0].Part, partLookup, flangeCollection);
            AddConstraintsBetweenParts(cross, flange1_2, selectedFlanges[1].Part, partLookup, flangeCollection);
            AddConstraintsBetweenParts(cross, flange2_1, selectedFlanges[2].Part, partLookup, flangeCollection);
            AddConstraintsBetweenParts(cross, flange2_2, selectedFlanges[3].Part, partLookup, flangeCollection);
            AddConstraintsBetweenParts(cross, flange3_1, selectedFlanges[4].Part, partLookup, flangeCollection);
            AddConstraintsBetweenParts(cross, flange3_2, selectedFlanges[5].Part, partLookup, flangeCollection);
             */

            Console.WriteLine("Is there overlapping?: {0}", IsOverlapping(assemblyDoc, flangeCollection));


            int count = 0;
            foreach (Object obj in flangeCollection)
            {
                ++count;
            }
            Console.WriteLine(count);
            //a.AddAndConstraintParts("C:\\Users\\Liam\\Desktop\\Parts\\6cross.ipt", "C:\\Users\\Liam\\Desktop\\Parts\\4000177.ipt", inventorApp, assemblyDoc);

            //a.AddTestPart("C:\\Users\\Liam\\Desktop\\Parts\\tube.ipt", "C:\\Users\\Liam\\Desktop\\Parts\\6000004.ipt");

            

            if (restoreToDefaults())
            {
                return;
            }
            Console.Clear();
        }



        //***********************************INVENTOR FUNCTIONS*****************************************//

        // Function that changes the color of the cross to metal for downloading
        static bool changeColorToGrey()
        {
            Console.WriteLine("Would you like to change the tubing colors to default grey (y/n)?");
            char answer = Console.ReadLine()[0];
            if (answer == 'n')
            {
                return false;
            }
            string blankCrossFile = "C:\\Users\\Liam\\Desktop\\Parts\\blankCross.ipt";
            // Open up the color version of cross 
            PartDocument crossPartDoc = inventorApp.Documents.Open(crossFile, true) as PartDocument;

            // Save new version as a new part document to save the standard one
            crossPartDoc.SaveAs(blankCrossFile, true);

            PartDocument blankCrossDoc = inventorApp.Documents.Open(blankCrossFile, true) as PartDocument;

            ResetColorToDefault(blankCrossDoc);

            blankCrossDoc.Update();
            blankCrossDoc.Close(true);



            return true;
        }

        // Function that sets every face color in a part to grey for exporting
        static void ResetColorToDefault(PartDocument partDoc)
        {

            // Get the component definition
            PartComponentDefinition partCompDef = partDoc.ComponentDefinition;

            // Get the surface bodies collection
            SurfaceBodies surfaceBodies = partCompDef.SurfaceBodies;

            // Add the metal asset appearance to the part doc
            Asset oAsset = partDoc.Assets.Add(AssetTypeEnum.kAssetTypeAppearance, "Metal");
            Console.WriteLine(oAsset.Name);

            // Change each face to the metal color
            foreach (SurfaceBody sb in surfaceBodies)
            {
                foreach (Face oFace in sb.Faces)
                {
                    oFace.Appearance = oAsset;
                }
            }
        }

        // Function to add a pipe and flange to an assembly and apply mate constraints
        static ComponentOccurrence AddPartToAssembly(string partFileName, in AssemblyDocument assemblyDoc)
        {
            // Create transient geometry for positioning parts
            TransientGeometry transGeometry = inventorApp.TransientGeometry;

            // Add the part occurrence to the assembly
            ComponentOccurrence partOccurrence = assemblyDoc.ComponentDefinition.Occurrences.Add(partFileName, transGeometry.CreateMatrix());

            return partOccurrence;
        }

        // Adds a mate constraint between two components existing in the assembly document. Will add a planar mate for planar faces and 
        // a concentric mate for cylindrical surfaces. 
        static void AddConstraintsBetweenParts(ComponentOccurrence pipeOccurrence, ComponentOccurrence flangeOccurrence, string partNumber, in FlangeIdentifier oF, ObjectCollection fCollection)
        {
            fCollection.Add(flangeOccurrence);
            Console.WriteLine("Objects in collection: {0}", fCollection.Count);
            // Get hole to add flange to
            Console.WriteLine("Enter pipe hole number to add constraint to: ");
            string strHole = Console.ReadLine();
            int hole = Convert.ToInt32(strHole);

            // Get the inner and outer diameters of the flange seat 
            Diameters diameters = getFDiameters(oF);
            double iDiameter = diameters.InnerDiameter;
            double ofDiameter = diameters.OuterDiameter;
            //Diameters? tubeDiameters = GetClosestNotLarger(ofDiameter, tubingLookup);
            //double? oDiameter = tubeDiameters.Value.OuterDiameter;
            //double? inDiameter = tubeDiameters.Value.InnerDiameter;
            double tubeOD = inchesToCm(oF.Tube_OD);
            double tubeID = tubeOD - inchesToCm(0.065);




            // Get the area of the face the pipe is connecting to
            //double area = Math.PI * (Math.Pow(ofDiameter/2,2) - Math.Pow(iDiameter/2, 2));
            double area = oF.SB_Area;

            Console.WriteLine("Area of {0}: {1}", partNumber, area);
            // Get the part documents for pipe and flange
            PartDocument pipePartDoc = pipeOccurrence.Definition.Document as PartDocument;
            PartDocument flangePartDoc = flangeOccurrence.Definition.Document as PartDocument;

            // Find specific faces and edges on the pipe and flange parts
            Face flangeFace = FindSmallestFaceBySurfaceType(flangePartDoc.ComponentDefinition.SurfaceBodies, area);
            Face flangeCylinderFace = FindLargestFaceBySurfaceType(flangePartDoc.ComponentDefinition.SurfaceBodies, SurfaceTypeEnum.kCylinderSurface);
            Face pipeFace = FindColorFace(pipePartDoc, hole, SurfaceTypeEnum.kPlaneSurface);
            Face pipeCylinderFace = FindColorFace(pipePartDoc, hole, SurfaceTypeEnum.kCylinderSurface);

            // Get the assembly component definition
            AssemblyComponentDefinition assemblyCompDef = pipeOccurrence.Parent as AssemblyComponentDefinition;

            // Create geometry proxies for constraints
            Object pipeFaceProxy;
            pipeOccurrence.CreateGeometryProxy(pipeFace, out pipeFaceProxy);
            Object flangeFaceProxy;
            flangeOccurrence.CreateGeometryProxy(flangeFace, out flangeFaceProxy);
            Object pipeCylinderFaceProxy;
            pipeOccurrence.CreateGeometryProxy(pipeCylinderFace, out pipeCylinderFaceProxy);
            Object flangeFaceCylinderProxy;
            flangeOccurrence.CreateGeometryProxy(flangeCylinderFace, out flangeFaceCylinderProxy);

            // Create assembly constraints object
            AssemblyConstraints constraints = assemblyCompDef.Constraints;

            try
            {
                // Add mate constraints to position the pipe and flange correctly
                MateConstraint mateConstraint = constraints.AddMateConstraint(pipeFaceProxy, flangeFaceProxy, 0.0, InferredTypeEnum.kNoInference, InferredTypeEnum.kNoInference, null, null);
                MateConstraint mateConstraint1 = constraints.AddMateConstraint(pipeCylinderFaceProxy, flangeFaceCylinderProxy, 0.0, InferredTypeEnum.kInferredLine, InferredTypeEnum.kInferredLine, null, null);
            }
            catch (Exception ex)
            {
                // Log the error if unable to add mate constraints
                Console.WriteLine("Unable to add mate constraint: " + ex.Message);
            }

            string ID_param, OD_param;

            if (hole == 1 || hole == 2)
            {
                ID_param = "V_ID";
                OD_param = "V_OD";
            }
            else if (hole == 3 || hole == 4)
            {
                ID_param = "H_ID";
                OD_param = "H_OD";
            }
            else if (hole == 5 || hole == 6)
            {
                ID_param = "N_ID";
                OD_param = "N_OD";
            }
            else
                throw new InvalidOperationException("Invalid Parameter");
            UpdatePipeDiameters(ID_param, OD_param, 2, 2.5);

            IsOverlapping(assemblyDoc, fCollection);
        }

        static void AddConstraintsBetweenPartsOld(ComponentOccurrence pipeOccurrence, ComponentOccurrence flangeOccurrence, string partNumber, in Dictionary<string, Diameters> partDictionary, ObjectCollection fCollection)
        {
            fCollection.Add(flangeOccurrence);
            Console.WriteLine("Objects in collection: {0}", fCollection.Count);
            // Get hole to add flange to
            Console.WriteLine("Enter pipe hole number to add constraint to: ");
            string strHole = Console.ReadLine();
            int hole = Convert.ToInt32(strHole);

            // Get the inner and outer diameters of the flange seat 
            Diameters diameters = getFlangeDiameters(partDictionary, partNumber);
            double iDiameter = diameters.InnerDiameter;
            double ofDiameter = diameters.OuterDiameter;
            Diameters? tubeDiameters = GetClosestNotLarger(ofDiameter, tubingLookup);
            double? oDiameter = tubeDiameters.Value.OuterDiameter;
            double? inDiameter = tubeDiameters.Value.InnerDiameter;

            // Get the area of the face the pipe is connecting to
            double area = Math.PI * (Math.Pow(ofDiameter / 2, 2) - Math.Pow(iDiameter / 2, 2));
            Console.WriteLine("Area of {0}: {1}", partNumber, area);
            // Get the part documents for pipe and flange
            PartDocument pipePartDoc = pipeOccurrence.Definition.Document as PartDocument;
            PartDocument flangePartDoc = flangeOccurrence.Definition.Document as PartDocument;

            // Find specific faces and edges on the pipe and flange parts
            Face flangeFace = FindSmallestFaceBySurfaceType(flangePartDoc.ComponentDefinition.SurfaceBodies, area);
            Face flangeCylinderFace = FindLargestFaceBySurfaceType(flangePartDoc.ComponentDefinition.SurfaceBodies, SurfaceTypeEnum.kCylinderSurface);
            Face pipeFace = FindColorFace(pipePartDoc, hole, SurfaceTypeEnum.kPlaneSurface);
            Face pipeCylinderFace = FindColorFace(pipePartDoc, hole, SurfaceTypeEnum.kCylinderSurface);

            // Get the assembly component definition
            AssemblyComponentDefinition assemblyCompDef = pipeOccurrence.Parent as AssemblyComponentDefinition;

            // Create geometry proxies for constraints
            Object pipeFaceProxy;
            pipeOccurrence.CreateGeometryProxy(pipeFace, out pipeFaceProxy);
            Object flangeFaceProxy;
            flangeOccurrence.CreateGeometryProxy(flangeFace, out flangeFaceProxy);
            Object pipeCylinderFaceProxy;
            pipeOccurrence.CreateGeometryProxy(pipeCylinderFace, out pipeCylinderFaceProxy);
            Object flangeFaceCylinderProxy;
            flangeOccurrence.CreateGeometryProxy(flangeCylinderFace, out flangeFaceCylinderProxy);

            // Create assembly constraints object
            AssemblyConstraints constraints = assemblyCompDef.Constraints;

            try
            {
                // Add mate constraints to position the pipe and flange correctly
                MateConstraint mateConstraint = constraints.AddMateConstraint(pipeFaceProxy, flangeFaceProxy, 0.0, InferredTypeEnum.kNoInference, InferredTypeEnum.kNoInference, null, null);
                MateConstraint mateConstraint1 = constraints.AddMateConstraint(pipeCylinderFaceProxy, flangeFaceCylinderProxy, 0.0, InferredTypeEnum.kInferredLine, InferredTypeEnum.kInferredLine, null, null);
            }
            catch (Exception ex)
            {
                // Log the error if unable to add mate constraints
                Console.WriteLine("Unable to add mate constraint: " + ex.Message);
            }

            string ID_param, OD_param;

            if (hole == 1 || hole == 2)
            {
                ID_param = "V_ID";
                OD_param = "V_OD";
            }
            else if (hole == 3 || hole == 4)
            {
                ID_param = "H_ID";
                OD_param = "H_OD";
            }
            else if (hole == 5 || hole == 6)
            {
                ID_param = "N_ID";
                OD_param = "N_OD";
            }
            else
                throw new InvalidOperationException("Invalid Parameter");
            UpdatePipeDiameters(ID_param, OD_param, inDiameter.Value, oDiameter.Value);

            IsOverlapping(assemblyDoc, fCollection);
        }

        // Changes the inner and outer diameter of the tubes of the cross/T/pipe to fit the flange being connected to it
        static void UpdatePipeDiameters(string IDParam, string ODParam, double IDVal, double ODVal)
        {
            // Update cross.ipt diameters
            PartDocument crossPartDoc = inventorApp.Documents.Open(crossFile, true) as PartDocument;
            Parameters partParams = crossPartDoc.ComponentDefinition.Parameters;
            units = crossPartDoc.UnitsOfMeasure;
            Console.WriteLine(units.LengthUnits);

            Parameter ID = partParams[IDParam];
            Parameter OD = partParams[ODParam];
        
            ID.Value = IDVal;
            OD.Value = ODVal;
            crossPartDoc.Update();
            assemblyDoc.Update();
            crossPartDoc.Close(true);
        }

        // Function to change the length of the pipes that make up the cross/T/pipe
        static void UpdatePipeLength()
        {
            // Open up part document to edit
            PartDocument crossPartDoc = inventorApp.Documents.Open(crossFile, true) as PartDocument;
            Parameters partParams = crossPartDoc.ComponentDefinition.Parameters;
            units = crossPartDoc.UnitsOfMeasure;
            printColorCode(colorCodes);

            // Get pipe that needs to be updated
            Console.WriteLine("Enter the pipe number you want to change the length of: ");
            string strPipe = Console.ReadLine();
            int pipe = Convert.ToInt32(strPipe);

            Console.WriteLine("Enter desired length of pipe in inches: ");
            string strLength = Console.ReadLine();
            double lenInches = Convert.ToDouble(strLength);
            double lenCm = units.ConvertUnits(lenInches, 11272, 11268);

            string lengthParam;

            if(pipe == 1)
            {
                lengthParam = "L1";
            }
            else if (pipe == 2)
            {
                lengthParam = "L2";
            }
            else if (pipe == 3)
            {
                lengthParam = "L3";
            }
            else if (pipe == 4)
            {
                lengthParam = "L4";
            }
            else if (pipe == 5)
            {
                lengthParam = "L5";
            }
            else
            {
                lengthParam = "L6";
            }

            Parameter lengthParamVal = partParams[lengthParam];
            lengthParamVal.Value = lenCm;
            
            crossPartDoc.Update();
            assemblyDoc.Update();
            crossPartDoc.Close(true);
        }

        // Functions that checks whether flanges in the assembly document are overlapping
        static bool IsOverlapping(in AssemblyDocument assemblyDoc, ObjectCollection flangesCollection)
        {
            InterferenceResults results;
            results = assemblyDoc.ComponentDefinition.AnalyzeInterference(flangesCollection);

            if (results.Count == 1)
            {
                Console.WriteLine("There is 1 interference.");
                return true;
            }
            else if (results.Count > 1)
            {
                Console.WriteLine($"There are {results.Count} interferences.");
                return true;
            }

            Console.WriteLine($"There are {results.Count} interferences.");

            return false;
        }


        //***********************************PARSING PARTS*****************************************//


        // Finds and returns the face with the largest area that matches a given surface type from the surface bodies
        static Face FindLargestFaceBySurfaceType(SurfaceBodies surfaceBodies, SurfaceTypeEnum surfaceType)
        {
            Face largestFace = null;
            double largestArea = double.MinValue;

            foreach (Face face in surfaceBodies[1].Faces)
            {
                if (face.SurfaceType == surfaceType)
                {
                    double area = face.Evaluator.Area;
                    if (area > largestArea)
                    {
                        largestArea = area;
                        largestFace = face;
                    }
                }
            }
            return largestFace; // Return the largest matching face, or null if not found
        }

        // Finds and returns the face with the smallest area that matches a given surface type from the surface bodies
        static Face FindSmallestFaceBySurfaceType(SurfaceBodies surfaceBodies, double targetArea)
        {
            Face targetFace = null;
            double difference = double.MaxValue;

            foreach (Face face in surfaceBodies[1].Faces)
            {
                if (face.SurfaceType == SurfaceTypeEnum.kPlaneSurface)
                {
                    double area = face.Evaluator.Area;
                    if (Math.Abs(area - targetArea) < difference)
                    {
                        difference = Math.Abs(area - targetArea);
                        targetFace = face;
                    }
                }
            }
            Console.WriteLine($"Found Face {targetFace.Appearance.Name}");
            return targetFace; // Return the smallest matching face, or null if not found
        }

        // Finds and returns the first face that matches a given surface type from the surface bodies
        static Face FindFaceBySurfaceType(SurfaceBodies surfaceBodies, SurfaceTypeEnum surfaceType)
        {
            foreach (Face face in surfaceBodies[1].Faces)
            {
                if (face.SurfaceType == surfaceType)
                {
                    return face; // Return the matching face
                }
            }
            return null; // Return null if no matching face is found
        }

        // Finds and returns the first edge that matches a given curve type from the provided edges
        static Edge FindEdgeByCurveType(Edges edges, CurveTypeEnum curveType)
        {
            foreach (Edge edge in edges)
            {
                if (edge.GeometryType == curveType)
                {
                    return edge; // Return the matching edge
                }
            }
            return null; // Return null if no matching edge is found
        }

        // Function to get the relevant face corresponding to its color and number to assign constraints with
        static Face FindColorFace(PartDocument partDoc, int hole, SurfaceTypeEnum surfaceType)
        {
            string color = colorCodes[hole];

            // Get the component definition
            PartComponentDefinition partCompDef = partDoc.ComponentDefinition;

            // Get the surface bodies collection
            SurfaceBodies surfaceBodies = partCompDef.SurfaceBodies;

            // Iterate through surface bodies (should be one for a simple pipe)
            try
            {
                foreach (Face face in surfaceBodies[hole].Faces)
                {
                    if (face == null)
                    {
                        continue; // Skip the current iteration
                    }

                    if (face.SurfaceType != surfaceType)
                    {
                        continue;
                    }

                    string faceAppearanceName = face.Appearance.Name.ToString();
                    if (faceAppearanceName.Contains(color))
                    {
                        Console.WriteLine(face.Appearance.DisplayName);
                        return face;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }

        // Function to get the correct inner and outer diameters for the planar faces on the flanges to constrain the cross/T/pipe to. 

        static Diameters getFlangeDiameters(in Dictionary<string, Diameters> partDiameters, string partNumber)
        {
            Diameters diameters;

            if (partDiameters.TryGetValue(partNumber, out diameters))
            {
                Console.WriteLine($"Inner Diameter: {diameters.InnerDiameter}, Outer Diameter: {diameters.OuterDiameter}");
            }
            else
            {
                Console.WriteLine("Part number not found.");
            }

            return diameters;
        }

        static Diameters getFDiameters(FlangeIdentifier oF)
        {
            Diameters diameters = new Diameters();

            if (oF != null)
            {
                diameters.InnerDiameter = oF.SB_ID;
                diameters.OuterDiameter = oF.SB_OD;

                Console.WriteLine($"Inner Diameter: {diameters.InnerDiameter}, Outer Diameter: {diameters.OuterDiameter}");
            }
            else
            {
                Console.WriteLine("Part number not found.");
            }

            return diameters;
        }


        //***********************************UTILITY FUNCTIONS*****************************************//

        static Dictionary<string, Diameters> generateLookupTable(string filename)
        {
            // Initialize dictionary of Flanges and their inner and outer diameters that the pipes fit to
            var partDiameters = new Dictionary<string, Diameters>();

            foreach (var line in System.IO.File.ReadAllLines(filename))
            {
                var parts = line.Split(' ');

                if (parts.Length == 3 && double.TryParse(parts[1], out double iDiameter) && double.TryParse(parts[2], out double oDiameter))
                {
                    partDiameters[parts[0]] = new Diameters
                    {
                        InnerDiameter = iDiameter,
                        OuterDiameter = oDiameter
                    };
                }
            }

            return partDiameters;
        }

        // Generates lookup table for the tubing diameters
        static Dictionary<string, Diameters> generateTubingLookupTable(string filename)
        {
            // Initialize dictionary of tubing part numbers and their inner and outer diameters 
            var tubeDiameters = new Dictionary<string, Diameters>();

            foreach (var line in System.IO.File.ReadAllLines(filename))
            {
                var parts = line.Split(' ');

                if (parts.Length == 3 && double.TryParse(parts[1], out double oDiameter) && double.TryParse(parts[2], out double wallThickness))
                {
                    //Console.WriteLine(oDiameter + " " + wallThickness);
                    tubeDiameters[parts[0]] = new Diameters
                    {
                        InnerDiameter = (oDiameter - 2 * wallThickness),
                        OuterDiameter = oDiameter
                    };
                    //Console.WriteLine($"Part: {parts[0]}, OD: {oDiameter}, Wall: {wallThickness}");

                }
            }

            return tubeDiameters;
        }

        // Prints the surface area and face type to the command console
        static void PrintFaceInfo(string partName, Face face)
        {
            if (face != null)
            {
                Console.WriteLine($"{partName} Face Area: " + face.Evaluator.Area);
                Console.WriteLine($"{partName} Face SurfaceType: " + face.SurfaceType);
            }
            else
            {
                Console.WriteLine($"{partName} face not found!");
            }
        }

        // Get paramter for inner and outer diameter from user in inches and return converted to cm
        static float getPar(string prompt)
        {
            Console.Write(prompt + ": "); // Display relevant prompt for measurement
            float input = float.Parse(Console.ReadLine()); // Get user input for dimension
            return (float)units.ConvertUnits(input, 11272, 11268); //convert inches to cm and return
        }

        // Prints the colors assosciated with the tube number on the cross
        static void printColorCode(in Dictionary<int, string> colorCodes)
        {
            foreach (KeyValuePair<int, string> ele2 in colorCodes)
            {
                Console.WriteLine("{0}: {1}", ele2.Key, ele2.Value);
            }
        }

        // Function that updates the cross file to its original parameters
        static bool restoreToDefaults()
        {
            Console.WriteLine("Would you like to restore to defaults and clear the assembly document (y/n)?");
            char answer = Console.ReadLine()[0];
            if (answer == 'n')
            {
                return false;
            }

            PartDocument crossPartDoc = inventorApp.Documents.Open(crossFile, true) as PartDocument;
            Parameters partParams = crossPartDoc.ComponentDefinition.Parameters;
            units = crossPartDoc.UnitsOfMeasure;

            foreach(Parameter param in  partParams)
            {
                if (param.Comment != "")
                {
                    param.Value = units.ConvertUnits(Convert.ToDouble(param.Comment), 11272, 11268);

                }
                
            }

            //ResetColorToDefault(crossPartDoc);

            crossPartDoc.Update();
            //crossPartDoc.Close(true);

            

            return true;
        }



        // Function that returns the tubing part that has the closest outer diameter to the flange without being wider than the setback
        static Diameters? GetClosestNotLarger(double outerDiameter, Dictionary<string, Diameters> tubingLookup)
        {
            //KeyValuePair<string, Diameters>? previousPair = null;
            Diameters? previousDiameters = null;
            foreach (var pair in tubingLookup)
            {
                if (pair.Value.OuterDiameter > outerDiameter)
                {
                    // If there's no previous pair, it means the provided diameter is smaller than the smallest in the dictionary.
                    return previousDiameters;
                }
                previousDiameters = pair.Value;
            }

            // If we haven't returned yet, then the provided diameter is larger than all in the dictionary, so return the last one.
            return previousDiameters;
        }

        // Converts inches to cm
        static double inchesToCm(double inches)
        {
            return inches * 2.5400;
        }


    }

    


    // Define a structure for reading flange inner and outer diameters for fitting pipe sizes
    public struct Diameters
    {
        public double InnerDiameter { get; set; }
        public double OuterDiameter { get; set; }
    }

    // Defines a structure for a Flange object to store information on flanges that can be added to assembly 
    public struct Flange
    {
        public string partNumber { get; set; }
        public string label { get; set; }
        public double setBackDiameter { get; set; }
        public double setBackArea { get; set; }
    }
    
}
