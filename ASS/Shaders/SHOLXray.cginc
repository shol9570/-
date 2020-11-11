/* SHOLXray
 * Author : SHOL
 * Description : SHOL's X-ray shader
 * Notice : Require "Tags{ "RenderType" = "Transparent" "Queue" = "Transparent" "IgnoreProjector" = "True" "LightMode"="ForwardBase" }"
*          and "Cull off, ZWrite On, Blend SrcAlpha OneMinusSrcAlpha".
*          Need "_Center, _Radius, _Raydir" properties.
 */

#ifndef SHOL_XRAY_INCLUDED
#define SHOL_XRAY_INCLUDED

#include "UnityCG.cginc"

inline bool IsInRange(float3 _target, float3 _center, float _radius)
{
    if(distance(_center, _target) < _radius) return true;
    return false;
}

inline bool IsFacing(float3 _normal, float3 _direction)
{
    if(dot(_normal, _direction) < 0) return true;
    return false;
}


#endif //SHOL_XRAY_INCLUDED