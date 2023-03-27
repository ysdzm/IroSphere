# HSLColorSphere

HSLColorSphere


■概要

画像の色を参照し、ＨＳＬ色空間に配置します。


■簡単な使い方

・Projectウィンドウの「Scenes > SampleScene」を開きます。

・画面上のPlayボタンでゲーム実行します

・左の写真をクリックすると、対応した箇所の座標に球が作成されます

・球は、「WASD」「十字キー」「コントローラーの左スティック」で回転可能です


■画像を差し替えたい時

※以下の操作は必ずゲームを止めた状態で行って下さい

・画像を差し替えたいときは、Hierarchyの「Canvas > image」を選択し、Inspectorの「Image > SourceImage」に画像をドラッグ・アンド・ドロップしてください。

　画像はProjectウィンドウのTexturesフォルダにサンプルが何枚か入っています。
 
・画像のサイズは「Canvas > image」を選択し、Inspectorの「Rect Transform > Width / Height」で変更してください。

・自分の画像を使いたい時は、好きな画像をエクスプローラーからProjectウィンドウの「Textures」フォルダにドラッグ・アンド・ドロップしてインポートし、
　その画像のInspectorで「TextureType」 を「Sprite(2D and UI)」に変更します。あとは同じ操作です。
 
 
■パラメーター調整方法

※以下の操作は必ずゲームを止めた状態で行って下さい

・「Hierarchy」を選択し、Inspectorの「SphereManger」で調整可能です。

　パラメーターの意味はマウスを重ねて一定時間待つ事で表示されます
 
 
  ■ライセンスについて
  
CC0です。ご自由にどうぞ
