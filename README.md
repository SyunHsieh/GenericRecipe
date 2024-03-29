﻿# GenericRecipe
GenericRecipe Class 旨在解決因頻繁的算法跌代，導致Recipe、介面及程式碼合併的頻繁更動及維護，主要實現內容如下

    a.區分暫存(通常為算法輸出結果)和永久儲存(通常為 Recipe 檢測參數)的參數定義為 Storable 和 Unstorable 兩個類別。
    b.使用 JSON 序列化保存/讀取 Recipe。
    c.定義 GenericTupl 類別，可保存以下參數類別：
      Int16、Int32、Int64、Single、Double、String、GenericDictionary、Bitmap
    d.定義 GenericInspectionParam 類別，用於定義不同 Recipe 行為模式。
    e.定義 IAlgorithmClass 介面，用於不同算法實例化。
    f.使用反射在運行時獲取 IAlgorithmClass 類別。
   <hr>
    相關優化如下圖表，顯示優化前後差異
    
   ![Alt Text](https://user-images.githubusercontent.com/8532735/218645012-a7bee6e4-841a-4495-98b7-b884a5cb7a9c.jpg)

