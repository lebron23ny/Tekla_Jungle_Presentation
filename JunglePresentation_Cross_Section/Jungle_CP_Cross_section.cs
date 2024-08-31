using System;
using Tekla.Structures.DrawingPresentationModel;
using Tekla.Structures.DrawingPresentationPluginInterface;
using System.ComponentModel.Composition;
using TSM = Tekla.Structures.Model;
using Tekla.Common.Geometry;
using TS = Tekla.Structures;
using TSD = Tekla.Structures.Drawing;
using TSG = Tekla.Structures.Geometry3d;
using JunglePresentation_Cross_Section.Tools;
using System.Collections.Generic;

namespace JunglePresentation_Cross_Section
{

    [Export(typeof(IDrawingPresentationPlugin))]
    [ExportMetadata("ForObjects", "JUNGLE_CP_CROSS_SECTION")]
    public class Jungle_CP_Cross_section : IDrawingPresentationPlugin
    {

        private TSM.Model _Model = new TSM.Model();
        private TSM.Part _Part;

        private Vector2 _CanvasMaxPoint;
        private Vector2 _CanvasMinPoint;

        private Segment _Presentation;

        private Pen Pen { get; set; }
        private Brush Brush { get; set; }
        private int GroupType { get; set; }

        public Segment CreatePresentation(Segment segment)
        {
            _Presentation = segment;
            SetPartBySegment();


            #region Определение координат Canvas
            LinePrimitive primitive1 = ((PrimitiveGroup)_Presentation).Primitives[0]
                            as LinePrimitive;
            LinePrimitive primitive2 = ((PrimitiveGroup)this._Presentation).Primitives[2]
                        as LinePrimitive;
            if (primitive1 != null && primitive2 != null)
            {
                _CanvasMinPoint = primitive1.StartPoint;
                _CanvasMaxPoint = primitive2.StartPoint;
            }
            #endregion
            Pen = new Pen(((int)TSD.DrawingColors.Red), ((int)TSD.LineTypes.SolidLine), 2);
            Brush = ((PrimitiveGroup)segment).Brush;
            GroupType = ((PrimitiveGroup)segment).GroupType;
            _Presentation = new Segment(((PrimitiveGroup)segment).Id, Pen, Brush, GroupType, segment.Layer, segment.ObjectType);
            Graph graph = new Graph(_Part, _CanvasMaxPoint, _CanvasMinPoint);
            List<PrimitiveBase> primitives = graph.GetLinePrimitives();
            foreach (PrimitiveBase primitive in primitives)
            {
                ((PrimitiveGroup)_Presentation).Primitives.Add(primitive);
            }
            return _Presentation;
        }


        /// <summary>
        /// Return TSM.Part by Presentation
        /// </summary>
        private void SetPartBySegment()
        {
            _Part = _Model.SelectModelObject(new TS.Identifier(((PrimitiveGroup)_Presentation).Id)) as TSM.Part;
        }
    }
}
