# DragDropClippedCanvas

WPF 애플리케이션에서 사용할 수 있는 고성능 드래그 앤 드롭 기능을 갖춘 재사용 가능한 캔버스 컨트롤입니다. 이 컴포넌트는 MVVM 패턴을 준수하고 다양한 요소들의 배치, 크기 조절 및 드래그 앤 드롭을 쉽게 구현할 수 있습니다.

## 주요 기능

### 재사용 가능한 DraggableCanvas 컨트롤

- **사용자 정의 크기**: 바인딩을 통해 너비와 높이를 동적으로 조절 가능 (기본 1024x768)
- **드래그 앤 드롭 지원**: 외부에서 캔버스로, 캔버스 내에서의 요소 드래그 지원
- **파일 드랍 지원**: 이미지 파일을 캔버스에 드롭하여 추가 가능
- **요소 타입 필터링**: 특정 타입의 요소만 허용하도록 설정 가능
- **이벤트 시스템**: 요소 추가, 이동, 크기 변경 시 이벤트 발생

### 요소 관리

- **경계 제한**: 모든 요소가 캔버스 경계를 벗어나지 않도록 제한
- **마우스 휠 크기 조절**: 요소에 마우스를 올리고 휠을 사용하여 크기 조절
- **중심점 기준 크기 조절**: 요소의 중심점을 유지하며 크기 변경
- **정사각형 비율 유지**: 크기 조절 시 요소의 정사각형 비율 유지
- **최소/최대 크기 제한**: 요소의 최소 크기(10x10)와 최대 크기(원래 크기 + 100) 제한

### MVVM 아키텍처

- **ViewModelBase**: INotifyPropertyChanged 구현으로 데이터 바인딩 지원
- **MainWindowViewModel**: 캔버스 크기, 상태 메시지, 명령 제공
- **RelayCommand**: ICommand 인터페이스 구현으로 XAML에서 명령 바인딩 지원

## 컴포넌트 구조

### DraggableCanvas (UserControl)

캔버스 요소들을 관리하는 주요 사용자 정의 컨트롤입니다.

#### 주요 속성

- `CanvasWidth`: 캔버스 너비 (DependencyProperty, 기본값 1024)
- `CanvasHeight`: 캔버스 높이 (DependencyProperty, 기본값 768)
- `ClipElementsToCanvas`: 요소들이 캔버스 경계를 벗어나지 않도록 할지 여부 (기본값 true)
- `MaintainSquareAspectRatio`: 요소의 정사각형 비율을 유지할지 여부 (기본값 true)
- `AllowedElementTypes`: 캔버스에 허용되는 요소 타입 목록

#### 주요 이벤트

- `ElementDropped`: 요소가 캔버스에 추가될 때 발생
- `ElementDragged`: 요소가 캔버스 내에서 드래그될 때 발생
- `ElementResized`: 요소의 크기가 변경될 때 발생

#### 주요 메서드

- `AddElement(UIElement element, Point position)`: 새 요소를 캔버스에 추가
- `ClearCanvas()`: 캔버스의 모든 요소 제거
- `MakeElementDraggable(UIElement element)`: 요소를 드래그 가능하게 설정

### MainWindowViewModel

메인 윈도우의 데이터 및 명령을 관리하는 ViewModel입니다.

#### 주요 속성

- `CanvasWidth`: 캔버스 너비
- `CanvasHeight`: 캔버스 높이
- `StatusMessage`: 상태 메시지 표시
- `ClearCanvasCommand`: 캔버스 초기화 명령
- `AddSampleElementCommand`: 샘플 요소 추가 명령

#### 주요 이벤트

- `ElementDropped`: ViewModel이 새 요소를 생성할 때 발생

## 사용 방법

### 기본 사용법

```xml
<local:DraggableCanvas
    x:Name="MyCanvas"
    CanvasWidth="1024"
    CanvasHeight="768"
    ClipElementsToCanvas="True" />
```

### 데이터 바인딩 사용

```xml
<local:DraggableCanvas
    x:Name="MyCanvas"
    CanvasWidth="{Binding CanvasWidth}"
    CanvasHeight="{Binding CanvasHeight}"
    ClipElementsToCanvas="True" />
```

### 요소 추가 (코드)

```csharp
Rectangle rect = new Rectangle
{
    Width = 100,
    Height = 100,
    Fill = Brushes.Blue
};

MyCanvas.AddElement(rect, new Point(50, 50));
```

### 이벤트 처리

```csharp
MyCanvas.ElementDropped += (sender, e) => {
    StatusText.Text = $"Element dropped at ({e.Position.X}, {e.Position.Y})";
};

MyCanvas.ElementResized += (sender, e) => {
    StatusText.Text = $"Element resized to {e.Width}x{e.Height}";
};
```

## 드래그 앤 드롭 기능

### 외부에서 캔버스로 드래그 앤 드롭

캔버스는 외부 소스(예: 파일 시스템)에서 요소(특히 이미지)를 드래그하여 캔버스에 드롭할 수 있습니다. 이 기능은 `MainCanvas_Drop` 이벤트 핸들러에서 관리됩니다.

### 캔버스 내에서의 드래그 앤 드롭

캔버스에 있는 모든 요소는 마우스로 드래그하여 이동할 수 있습니다. 이 기능은 `Element_MouseLeftButtonDown`, `Element_MouseMove`, `Element_MouseLeftButtonUp` 이벤트 핸들러에서 관리됩니다.

## 크기 조절 기능

### 마우스 휠 크기 조절

요소 위에 마우스를 올리고(호버) 마우스 휠을 사용하여 크기를 조절할 수 있습니다:

- 휠 위로 스크롤: 요소 크기 증가 (+10px)
- 휠 아래로 스크롤: 요소 크기 감소 (-10px)

### 비율 및 제한

- 항상 정사각형 비율 유지 (너비 = 높이)
- 최소 크기: 10x10 픽셀
- 최대 크기: 원래 크기 + 100px (예: 기본 크기 100x100인 경우 최대 200x200)

### 중심점 기준 크기 조절

요소 크기 조절 시 중심점을 기준으로 확대/축소됩니다. 이렇게 하면 요소 크기가 변경되어도 중심 위치는 유지됩니다.

### 경계 처리

요소가 캔버스 경계에 가까이 있을 때:

1. 요소의 크기가 제한됩니다 (캔버스를 벗어나지 않음).
2. 필요한 경우 위치가 조정되어 항상 캔버스 내부에 머무릅니다.
3. 특히 좌측이나 위쪽에 가까울 때 특별한 처리를 통해 올바른 크기와 위치를 유지합니다.

## 캔버스 경계 처리 알고리즘

캔버스의 주요 기능 중 하나는 요소들이 항상 캔버스 내에 머무르도록 하는 것입니다. 이를 위해 다음과 같은 단계별 알고리즘을 사용합니다:

1. 요소 크기 최소값(10px) 보장
2. 왼쪽 및 위쪽 경계 검사 및 조정
3. 오른쪽 및 아래쪽 경계 위반 확인
4. 사용 가능한 공간 계산
5. 최대 정사각형 크기 결정
6. 필요시 크기 조정
7. 중심점 기준 위치 재계산
8. 최종 경계 확인 및 추가 조정

이 알고리즘은 `AdjustSizeAndPositionForCanvas` 메서드에서 구현되어 있습니다.

## 사용 사례

이 컨트롤은 다음과, 같은 애플리케이션에서 유용하게 사용될 수 있습니다:

- 다이어그램 편집기
- 사진 콜라주 도구
- UI 디자인 도구
- 레이아웃 편집기
- 프레젠테이션 소프트웨어
- 간단한 그래픽 편집 도구
- 게임 레벨 디자이너

## 테스트 프로젝트 구조

프로젝트의 품질과 신뢰성을 보장하기 위해 MSTest를 기반으로 하는 단위 테스트 프로젝트가 포함되어 있습니다.

```
DragDropClippedCanvas.Tests/
├── Commands/
│   └── RelayCommandTests.cs           # RelayCommand 테스트
├── Controls/
│   └── DraggableCanvasTests.cs        # DraggableCanvas 테스트
├── ViewModels/
│   ├── MainWindowViewModelTests.cs    # 메인 뷰모델 테스트
│   └── ViewModelBaseTests.cs          # 기본 뷰모델 테스트
└── TestUtilities/
    ├── MockElement.cs                 # UIElement 모의 구현
    ├── UIElementHelper.cs             # UI 요소 테스트 유틸리티
    └── TestEventArgs.cs               # 이벤트 인자 생성 유틸리티
```

### 테스트 유틸리티 클래스

#### MockElement

테스트 목적으로 사용되는 UIElement의 모의 구현입니다. 마우스 이벤트 시뮬레이션, 크기 조정, 위치 지정 등의 기능을 제공합니다.

```csharp
// 마우스 이벤트 시뮬레이션 예시
var mockElement = new MockElement(100, 100);
var eventArgs = new MockMouseButtonEventArgs(new Point(50, 50));
mockElement.RaiseMouseLeftButtonDown(eventArgs);
```

#### UIElementHelper

Canvas와 UIElement 간의 상호작용을 테스트하기 위한 유틸리티 메서드를 제공합니다.

```csharp
// 테스트용 캔버스 생성
var canvas = UIElementHelper.CreateCanvas(1024, 768);

// 요소 위치 설정 및 확인
UIElementHelper.SetCanvasLeft(element, 50);
double left = UIElementHelper.GetCanvasLeft(element);
```

#### TestEventArgs

DraggableCanvas의 다양한 이벤트에 사용되는 이벤트 인자 객체를 생성하는 유틸리티 클래스입니다.

```csharp
// 요소 드롭 이벤트 인자 생성
var args = TestEventArgs.CreateElementDroppedEventArgs(element, new Point(50, 50));
```

### 주요 테스트 케이스

#### RelayCommandTests

RelayCommand 클래스의 기능을 검증하는 테스트입니다.

- 명령 실행 기능
- 매개변수 전달
- 실행 조건 검사
- 이벤트 발생 확인

#### DraggableCanvasTests

DraggableCanvas 컴포넌트의 핵심 기능을 검증하는 테스트입니다.

- 요소 추가
- 드래그 기능
- 캔버스 초기화
- 마우스 휠을 통한 크기 조절

#### ViewModelTests

ViewModel 클래스들의 기능을 검증하는 테스트입니다.

- 속성 변경 통지
- 데이터 바인딩
- 명령 실행
- 이벤트 발생

### 테스트 실행 방법

1. Visual Studio에서 테스트 탐색기 열기 (보기 -> 테스트 탐색기)
2. 솔루션 빌드 (빌드 -> 솔루션 빌드)
3. 테스트 탐색기에서 실행할 테스트 선택
4. "선택한 테스트 실행" 또는 "모든 테스트 실행" 클릭

또는 명령줄에서 다음과 같이 실행할 수 있습니다:

```
vstest.console.exe DragDropClippedCanvas.Tests.dll
```

## 구현 세부 사항

### 주요 클래스

#### DraggableCanvas

캔버스 컨트롤의 핵심 클래스로, 요소들의 배치, 상호작용, 크기 조절을 관리합니다.

#### MainWindowViewModel

애플리케이션의 메인 ViewModel로, 캔버스 크기 및 상태 관리를 담당합니다.

#### RelayCommand

명령 패턴을 구현하여 UI 액션을 ViewModel 메서드에 연결합니다.

### 주요 이벤트 인자 클래스

#### ElementDroppedEventArgs

요소가 캔버스에 추가될 때 발생하는 이벤트의 인자입니다.

- `Element`: 추가된 UI 요소
- `Position`: 요소가 추가된 위치

#### ElementDraggedEventArgs

요소가 드래그될 때 발생하는 이벤트의 인자입니다.

- `Element`: 드래그된 UI 요소
- `Position`: 현재 위치
- `Delta`: 이동 변화량(Vector)

#### ElementResizedEventArgs

요소 크기가 변경될 때 발생하는 이벤트의 인자입니다.

- `Element`: 크기가 변경된 UI 요소
- `Width`: 새 너비
- `Height`: 새 높이
- `Position`: 현재 위치

## 프로젝트 구조

```
DragDropClippedCanvas/
├── Commands/
│   └── RelayCommand.cs        # ICommand 구현
├── Controls/
│   ├── DraggableCanvas.xaml   # 드래그 앤 드롭 캔버스 UI
│   └── DraggableCanvas.xaml.cs # 캔버스 기능 구현
├── ViewModels/
│   ├── ViewModelBase.cs       # 기본 ViewModel
│   └── MainWindowViewModel.cs # 메인 화면 ViewModel
├── MainWindow.xaml            # 애플리케이션 메인 화면
└── MainWindow.xaml.cs         # 메인 화면 코드

DragDropClippedCanvas.Tests/
├── Commands/
│   └── RelayCommandTests.cs           # RelayCommand 테스트
├── Controls/
│   └── DraggableCanvasTests.cs        # DraggableCanvas 테스트
├── ViewModels/
│   ├── MainWindowViewModelTests.cs    # 메인 뷰모델 테스트
│   └── ViewModelBaseTests.cs          # 기본 뷰모델 테스트
└── TestUtilities/
    ├── MockElement.cs                 # UIElement 모의 구현
    ├── UIElementHelper.cs             # UI 요소 테스트 유틸리티
    └── TestEventArgs.cs               # 이벤트 인자 생성 유틸리티
```

## 확장 가능성

이 컨트롤은 다음과 같은 방향으로 확장할 수 있습니다:

1. 다양한 도형 생성 도구 추가
2. 요소 그룹화 기능
3. 요소 회전 기능
4. 레이어 지원
5. 실행 취소/다시 실행 기능
6. 저장 및 불러오기 기능
7. 그리드 및 스냅 기능
8. 다양한 테마 지원
9. 다중 선택 기능
10. 키보드 단축키 지원

## 요약

DragDropClippedCanvas는 WPF 애플리케이션에서 사용할 수 있는 강력하고 재사용 가능한 캔버스 컨트롤입니다. 드래그 앤 드롭, 크기 조절, 경계 제한 등 다양한 기능을 제공하며, MVVM 패턴을 준수하여 쉽게 통합할 수 있습니다. 또한 MSTest 기반의 테스트 프로젝트를 통해 각 컴포넌트의 기능과 신뢰성이 검증되었습니다. 다양한 사용 사례에 적용할 수 있으며, 확장 가능한 유연한 아키텍처를 가지고 있습니다.
