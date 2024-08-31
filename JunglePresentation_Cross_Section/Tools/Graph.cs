using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSM = Tekla.Structures.Model;
using Tekla.Common.Geometry;
using TSMUI = Tekla.Structures.Model.UI;
using TSG = Tekla.Structures.Geometry3d;
using TSS = Tekla.Structures.Solid;
using TS = Tekla.Structures;
using Tekla.Structures.DrawingPresentationModel;

namespace JunglePresentation_Cross_Section.Tools
{
    public class Graph
    {

        double widthCanvas;
        double heightCanvas;
        TSG.Vector vectorMove;
        List<TSG.Point> listSection;

        public Graph(TSM.Part part, Vector2 _CanvasMaxPoint, Vector2 _CanvasMinPoint)
        {
            widthCanvas = _CanvasMaxPoint.X - _CanvasMinPoint.X - 2; //Получаем высоту Canvas
            heightCanvas = _CanvasMaxPoint.Y - _CanvasMinPoint.Y - 2; //Получаем ширину Canvas

            vectorMove = new TSG.Vector((_CanvasMaxPoint.X + _CanvasMinPoint.X)/2, 
                (_CanvasMaxPoint.Y + _CanvasMinPoint.Y) / 2, 0); //Получаем вектор перемещения в центр Canvas


            List<TSG.Point> listSection = getSectionInCSpart(part); //Получаем точки поперечного сечения в системе координа детали
            double heightSection = getHeightSection(listSection); //Получаем высоту сечения
            double widthSection = getWidthSection(listSection); //Получаем ширину сечения
            double scaleFactor = scaleFactorSectionToCanvas(heightSection, widthSection, heightCanvas, widthCanvas); //Получаем размерный фактор
            this.listSection = getListSectionToCanvas(listSection, scaleFactor, vectorMove); //Получаем точки поперечного сечения отмасштабированные на Canvas и смещенные в центр Canvas

        }


        private List<TSG.Point> getSectionInCSpart(TSM.Part part)
        {
            List<TSG.Point> listPointCrossSection = new List<TSG.Point>();

            if (part is TSM.Beam)
            {
                TSG.CoordinateSystem csPart = part.GetCoordinateSystem();

                TSG.Point originCsPart = csPart.Origin;
                TSG.Vector vectorXpart = csPart.AxisX;
                TSG.Vector vectorYpart = csPart.AxisY;
                TSG.Vector vectorZpart = vectorXpart.Cross(vectorYpart);
                TSG.Matrix matrix = TSG.MatrixFactory.ToCoordinateSystem(
                    new TSG.CoordinateSystem(originCsPart, vectorZpart, vectorYpart));
                TSM.Solid solidPart = part.GetSolid();
                TSS.FaceEnumerator faceEnumerator = solidPart.GetFaceEnumerator();
                List<TSS.Face> faces = new List<TSS.Face>();
                while (faceEnumerator.MoveNext())
                {
                    TSS.Face face = faceEnumerator.Current;
                    TSG.Vector normalVector = face.Normal;
                    TS.Identifier identifierFace = face.OriginPartId;

                    if (TSG.Parallel.VectorToVector(vectorXpart, normalVector) && (TSG.Vector.Dot(vectorXpart, normalVector) < 0))
                    {
                        faces.Add(face);
                        TSS.LoopEnumerator loopEnumerator = face.GetLoopEnumerator();
                        loopEnumerator.MoveNext();
                        TSS.Loop loop = loopEnumerator.Current;
                        TSS.VertexEnumerator vertexEnumerator = loop.GetVertexEnumerator();
                        while (vertexEnumerator.MoveNext())
                            listPointCrossSection.Add(matrix.Transform(vertexEnumerator.Current as TSG.Point));
                    }
                }
            }
            return listPointCrossSection;
        }

        private double getHeightSection(List<TSG.Point> listSection)
        {
            double maxY = listSection.Max(point => point.Y);
            double minY = listSection.Min(point => point.Y);
            return Math.Round(maxY - minY, 3);
        }

        private double getWidthSection(List<TSG.Point> listSection)
        {
            double maxX = listSection.Max(point => point.X);
            double minX = listSection.Min(point => point.X);
            return Math.Round(maxX - minX, 3);
        }


        private double scaleFactorSectionToCanvas(double heightSection, double widthSection, 
            double heightCanvas, double widthCanvas)
        {
            double scaleFactorWidth = widthCanvas / widthSection;
            double scaleFactorHeight = heightCanvas / heightSection;
            return Math.Min(scaleFactorWidth, scaleFactorHeight);
        }


        private List<TSG.Point> getListSectionToCanvas(
            List<TSG.Point> listSection, 
            double scaleFactor, 
            TSG.Vector vectorMove)
        {
            List<TSG.Point> listPoint = new List<TSG.Point>();
            foreach (TSG.Point point in listSection)
            {
                listPoint.Add(new TSG.Point(
                    point.X * scaleFactor + vectorMove.X,
                    point.Y * scaleFactor + vectorMove.Y,
                    point.Z * scaleFactor + vectorMove.Z));
            }
            return listPoint;
        }

        public List<PrimitiveBase> GetLinePrimitives()
        {
            List<PrimitiveBase> listPrimitives = new List<PrimitiveBase>();
            for(int i = 0; i<(listSection.Count - 1); i++)
            {
                Vector2 pt1 = new Vector2(listSection[i].X, listSection[i].Y);
                Vector2 pt2 = new Vector2(listSection[i+1].X, listSection[i+1].Y);
                LinePrimitive linePrimitive = new LinePrimitive(pt1, pt2);
                listPrimitives.Add(linePrimitive);
            }
            Vector2 ptFirst = new Vector2(listSection[0].X, listSection[0].Y);
            Vector2 ptLast = new Vector2(listSection[listSection.Count-1].X, listSection[listSection.Count-1].Y);
            LinePrimitive linePrimitiveLast = new LinePrimitive(ptLast, ptFirst);
            listPrimitives.Add(linePrimitiveLast);
            return listPrimitives;
        } 
    }
}
