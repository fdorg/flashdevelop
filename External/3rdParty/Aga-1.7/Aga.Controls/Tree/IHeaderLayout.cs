﻿// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
namespace Aga.Controls.Tree
{
  internal interface IHeaderLayout
  {
    int PreferredHeaderHeight
    {
      get;
      set;
    }

    void ClearCache();
  }
}
