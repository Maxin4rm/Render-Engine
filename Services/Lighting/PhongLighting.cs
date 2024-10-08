using DrawPolygons.Models.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrawPolygons.Services.Lighting
{
    public static class PhongLighting
    {
        private static VectorFourCoord TakeAmbientForPoint(float coefficientLightBackground, VectorFourCoord colorLightBackground)
        {
            return colorLightBackground.MultiplyWithVectorResultWithoutW(coefficientLightBackground);
        }

        private static VectorFourCoord TakeDiffuseForPoint(VectorFourCoord vectorLightToPoint, VectorFourCoord vectorPointNormal, 
            float coefficientLightDiffuse, VectorFourCoord colorLightDiffuse)
        {           
            float productCoefficientAndAngle = coefficientLightDiffuse * Math.Max(vectorPointNormal.
                MultiplyWithScalarResultWithoutW(vectorLightToPoint), 0.0f);

            return colorLightDiffuse.MultiplyWithVectorResultWithoutW(productCoefficientAndAngle);
        }

        private static VectorFourCoord TakeSpecularForPoint(VectorFourCoord vectorLightToPoint, VectorFourCoord vectorPointNormal, 
            VectorFourCoord vectorEyeToPoint, float coefficientLightDistributeSpecular, float coefficientLightSpecular, VectorFourCoord colorLightSpecular)
        {
            VectorFourCoord vectorLightReflectDirection = vectorLightToPoint.SubstractWithVectorWithoutW(vectorPointNormal.
                MultiplyWithVectorResultWithoutW(2.0f * Math.Max(vectorLightToPoint.MultiplyWithScalarResultWithoutW(vectorPointNormal), 0.0f))).TakeNormalizedVectorWithoutW();

            float prepareProductSpecularValue = coefficientLightSpecular * (float) Math.Pow(Math.Abs(
                Math.Min(vectorLightReflectDirection.MultiplyWithScalarResultWithoutW(vectorEyeToPoint), 0.0f)), 
                coefficientLightDistributeSpecular);

            return colorLightSpecular.MultiplyWithVectorResultWithoutW(prepareProductSpecularValue);
        }

        public static VectorFourCoord TakePhongLightingColorForPointShortened(VectorFourCoord vectorLightToPoint, VectorFourCoord vectorPointNormal, 
            float coefficientLightBackground, VectorFourCoord colorLightBackground, float coefficientLightDiffuse, VectorFourCoord colorLightDiffuse, 
            VectorFourCoord vectorEyeToPoint, float coefficientLightDistributeSpecular, float coefficientLightSpecular, VectorFourCoord colorLightSpecular)
        {
            VectorFourCoord colorAmbient = TakeAmbientForPoint(coefficientLightBackground, colorLightBackground);
            VectorFourCoord colorDiffuse = TakeDiffuseForPoint(vectorLightToPoint, vectorPointNormal, coefficientLightDiffuse, colorLightDiffuse);
            VectorFourCoord colorSpecular = TakeSpecularForPoint(vectorLightToPoint, vectorPointNormal, vectorEyeToPoint, coefficientLightDistributeSpecular, 
                coefficientLightSpecular, colorLightSpecular);

            return new VectorFourCoord(Math.Min(colorAmbient.X + colorDiffuse.X + colorSpecular.X, 1), Math.Min(colorAmbient.Y + colorDiffuse.Y + colorSpecular.Y, 1),
                Math.Min(colorAmbient.Z + colorDiffuse.Z + colorSpecular.Z, 1));
        }
    }
}
