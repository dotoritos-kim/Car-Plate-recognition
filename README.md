# Car-Plate-recognition

자동차의 번호판을 OpenCV 라이브러리를 이용하여 추출하는 소스코드입니다.
주 처리는 ProcessClass.cs에서 진행됩니다.


전처리 순서
1. CvtColor
2. EqualizeHist
3. FastNlMeansDenoising
4. AdaptiveThreshold
5. Dilate
6. Erode

boundingRect.X > (SnakePlate.Width / 20) * 3 &&  
boundingRect.Y > (SnakePlate.Height / 20) * 6 &&  
boundingRect.X < (SnakePlate.Width / 20) * 17 &&  
boundingRect.Y < (SnakePlate.Height / 20) * 14 &&  
(SnakePlate.Width / 20) * 3 < boundingRect.X + boundingRect.Width &&  
(SnakePlate.Height / 20) * 6 < boundingRect.Y + boundingRect.Height &&  
(SnakePlate.Width / 20) * 17 > boundingRect.X + boundingRect.Width &&  
(SnakePlate.Height / 20) * 14 > boundingRect.Y + boundingRect.Height &&  
boundingRect.Width > 3 &&  
boundingRect.Height > 3 &&  
boundingRect.Width < 300 &&  
boundingRect.Height < 200 &&  
boundingRect.Width * boundingRect.Height > 400  

일정 비율의 관심영역 안에서 사각형의 최소, 최대 크기를 지정하여 Recognition 합니다.

Bubble Sort를 이용하여 인식한 영역들을 정렬합니다.
영역들 간의 기울기를 계산하여 조건에 부합하며 서로 어울려 있는 사각형인지 판별하여 FindRect 리스트에 추가합니다.
FindRect에 있는 모든 사각형 영역을 한개의 Mat에 조합하여 하나의 이미지를 만들어 냅니다.

라벨링을 통하여 의미가 있어 보이는 영역을 추출해냅니다.
