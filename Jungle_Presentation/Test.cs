
using System;
using System.ComponentModel.Composition;
using Tekla.Structures.DrawingPresentationModel;
using Tekla.Structures.DrawingPresentationPluginInterface;
using TSM = Tekla.Structures.Model;
using TSD = Tekla.Structures.Drawing;
using TSDT = Tekla.Structures.Drawing.Tools;
using TS = Tekla.Structures;
using TSP = Tekla.Structures.Plugins;
using Tekla.Common.Geometry;
using TSG = Tekla.Structures.Geometry3d;
using System.Collections.Generic;

namespace Jungle_Presentation
{
    [Export(typeof(IDrawingPresentationPlugin))]
    [ExportMetadata("ForObjects", "JUNGLE_SYMB")]
    public class Test : IDrawingPresentationPlugin
    {
        private TSM.Model _Model = new TSM.Model();
        private TSM.Part _Part;
        private TSD.DrawingObject _DrawingObject;
        private Segment _Presentation;

        private Vector2 _CanvasMaxPoint;
        private Vector2 _CanvasMinPoint;

        private Pen Pen { get; set; }

        private Brush Brush { get; set; }

        private int GroupType { get; set; }


        public Segment CreatePresentation(Segment segment)
        {

            SetPartBySegment();
            List<PrimitiveBase> primitivesCanvas = new List<PrimitiveBase>();

            foreach(var lineCanvas in _Presentation.Primitives)
            {
                primitivesCanvas.Add((PrimitiveBase)lineCanvas);
            }

            Pen = new Pen(((int)TSD.DrawingColors.Red), ((int)TSD.LineTypes.SolidLine), 2);
            Brush = ((PrimitiveGroup)segment).Brush;
            GroupType = ((PrimitiveGroup)segment).GroupType;

            _Presentation = new Segment(((PrimitiveGroup)segment).Id, Pen, Brush, GroupType, segment.Layer, segment.ObjectType);
            #region старый код

            foreach (PrimitiveBase primitive in DrawDiagonal(segment))
            {
                ((PrimitiveGroup)_Presentation).Primitives.Add(primitive);
            }


            foreach (PrimitiveBase primitive in DrawCenterVerticalLine(segment))
            {
                ((PrimitiveGroup)_Presentation).Primitives.Add(primitive);
            }

            foreach (PrimitiveBase primitive in DrawCenterHorizontalLine(segment))
            {
                ((PrimitiveGroup)_Presentation).Primitives.Add(primitive);
            }

            #endregion

            return _Presentation;
        }


        /// <summary>
        /// Return TSM.Part by Presentation
        /// </summary>
        private void SetPartBySegment()
        {
            _Part = _Model.SelectModelObject(new TS.Identifier(((PrimitiveGroup)_Presentation).Id)) as TSM.Part;
        }

        /// <summary>
        /// Returnt TSD.DrawingObject by Presentation
        /// </summary>
        private void SetDrawingObjectBySegment()
        {
            _DrawingObject = TSDT.InputDefinitionFactory.GetDrawingObject(new TSP.DrawingPluginBase.InputDefinition(
                new TS.Identifier(((PrimitiveGroup)_Presentation).Id),
                new TS.Identifier(((PrimitiveGroup)_Presentation).Id)
                )) as TSD.DrawingObject;
        }


        private List<PrimitiveBase> DrawCanvas(Segment segment)
        {
            List<PrimitiveBase> listPrimitives = new List<PrimitiveBase>();

            Vector2 pt1 = (((PrimitiveGroup)segment).Primitives[0] as LinePrimitive).StartPoint;
            Vector2 pt2 = (((PrimitiveGroup)segment).Primitives[0] as LinePrimitive).EndPoint;
            Vector2 pt3 = (((PrimitiveGroup)segment).Primitives[1] as LinePrimitive).EndPoint;
            Vector2 pt4 = (((PrimitiveGroup)segment).Primitives[2] as LinePrimitive).EndPoint;


            LinePrimitive linePrimitive1 = new LinePrimitive(pt1, pt2);
            LinePrimitive linePrimitive2 = new LinePrimitive(pt2, pt3);
            LinePrimitive linePrimitive3 = new LinePrimitive(pt3, pt4);
            LinePrimitive linePrimitive4 = new LinePrimitive(pt4, pt1);

            listPrimitives.Add((PrimitiveBase)linePrimitive1);
            listPrimitives.Add((PrimitiveBase)linePrimitive2);
            listPrimitives.Add((PrimitiveBase)linePrimitive3);
            listPrimitives.Add((PrimitiveBase)linePrimitive4);

            return listPrimitives;
        }


        private List<PrimitiveBase> DrawDiagonal(Segment segment)
        {
            List<PrimitiveBase> listPrimitives = new List<PrimitiveBase>();

            Vector2 pt1 = (((PrimitiveGroup)segment).Primitives[0] as LinePrimitive).StartPoint;
            Vector2 pt2 = (((PrimitiveGroup)segment).Primitives[0] as LinePrimitive).EndPoint;
            Vector2 pt3 = (((PrimitiveGroup)segment).Primitives[1] as LinePrimitive).EndPoint;
            Vector2 pt4 = (((PrimitiveGroup)segment).Primitives[2] as LinePrimitive).EndPoint;

            LinePrimitive linePrimitive1 = new LinePrimitive(pt1, pt3);
            //LinePrimitive linePrimitive2 = new LinePrimitive(pt2, pt4);

            listPrimitives.Add((PrimitiveBase)linePrimitive1);
            //listPrimitives.Add((PrimitiveBase)linePrimitive2);

            return listPrimitives;
        }


        private List<PrimitiveBase> DrawCenterVerticalLine(Segment segment)
        {
            List<PrimitiveBase> listPrimitives = new List<PrimitiveBase>();

            Vector2 pt1 = (((PrimitiveGroup)segment).Primitives[0] as LinePrimitive).StartPoint;
            Vector2 pt2 = (((PrimitiveGroup)segment).Primitives[0] as LinePrimitive).EndPoint;
            Vector2 pt3 = (((PrimitiveGroup)segment).Primitives[1] as LinePrimitive).EndPoint;
            Vector2 pt4 = (((PrimitiveGroup)segment).Primitives[2] as LinePrimitive).EndPoint;

            Vector2 middleBottom = (pt2 + pt1)/2;
            Vector2 middleTop = (pt3 + pt4)/2;

            LinePrimitive linePrimitive1 = new LinePrimitive(middleBottom, middleTop);
            //LinePrimitive linePrimitive2 = new LinePrimitive(pt2, pt4);

            listPrimitives.Add((PrimitiveBase)linePrimitive1);
            //listPrimitives.Add((PrimitiveBase)linePrimitive2);

            return listPrimitives;
        }


        private List<PrimitiveBase> DrawCenterHorizontalLine(Segment segment)
        {
            List<PrimitiveBase> listPrimitives = new List<PrimitiveBase>();

            Vector2 pt1 = (((PrimitiveGroup)segment).Primitives[0] as LinePrimitive).StartPoint;
            Vector2 pt2 = (((PrimitiveGroup)segment).Primitives[0] as LinePrimitive).EndPoint;
            Vector2 pt3 = (((PrimitiveGroup)segment).Primitives[1] as LinePrimitive).EndPoint;
            Vector2 pt4 = (((PrimitiveGroup)segment).Primitives[2] as LinePrimitive).EndPoint;

            Vector2 middleLeft = (pt4 + pt1) / 2;
            Vector2 middleRight = (pt3 + pt2) / 2;

            LinePrimitive linePrimitive1 = new LinePrimitive(middleLeft, middleRight);
            //LinePrimitive linePrimitive2 = new LinePrimitive(pt2, pt4);

            listPrimitives.Add((PrimitiveBase)linePrimitive1);
            //listPrimitives.Add((PrimitiveBase)linePrimitive2);

            return listPrimitives;
        }

    }
}
