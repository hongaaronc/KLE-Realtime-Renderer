using UnityEngine;
using System.Collections;

public class Glyph
{
    public Rect uv;
    public Rect vert;
    public float width;
    public int index;
}

public class SpriteFont : ScriptableObject
{
    public Glyph[] glyphs;
    public Texture2D sprite_texture;

    
}