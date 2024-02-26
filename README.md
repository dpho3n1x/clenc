# Clenc
App available only in Polish language.

Clenc jest prostą w obsłudze aplikacją służącą do łatwego i szybkiego **przycinania nagrań**. Pośród innych aplikacji wyróżnia się efektywnością. Pozwala na uzyskanie **bardzo dobrego stosunku jakości do wielkości pliku**. Zawdzięcza to nowoczesnym kodekom obrazu i dźwięku. 

## Wymagania aplikacji:
Aby aplikacja działała wymagane są biblioteki:
- FFMPEG (https://www.gyan.dev/ffmpeg/builds/ffmpeg-git-essentials.7z - umieść plik ffmpeg.exe w folderze aplikacji)
- Biblioteka Windows .NET 4.5.2

Zalecane dodatki:
- Dowolna paczka kodeków do Windows Media Player
- Odtwarzacz MPV (https://mpv.io/installation/)

Sprzęt:
- Nowoczesny procesor (znacznie przyspiesza konwersję)
- 80MB wolnego miejsca na dysku
- System operacyjny Windows Vista lub nowszy (64-bit)

## Jak korzystać z aplikacji:
1. Wybierz nagranie do przycięcia
![obraz](https://user-images.githubusercontent.com/57898662/114313235-01eeed80-9af6-11eb-990b-b9c7e7fb6763.png)

2. Wybierz czas początku i końca nagrania
![obraz](https://user-images.githubusercontent.com/57898662/114313284-437f9880-9af6-11eb-8e38-6ffdf1c77e37.png)

3. Wyznacz oczekiwaną wielkość nagrania (domyślnie 8MB pod Discorda). Następnie kliknij przycisk "Renderuj Nagranie"
![obraz](https://user-images.githubusercontent.com/57898662/114313358-7c1f7200-9af6-11eb-8a94-9b2d68c1b9ef.png)

## Kodeki używane przez aplikację:
Kodek obrazu: **VP9**
- Jest to nowoczesny kodek obrazu pozwalający na uzyskanie większej efektywności od klasycznego h264.
- Jest używany m.in. przez Google na YouTube, Netflixa oraz wielu innych platformach streamingowych
- Jest w pełni obsługiwany przez Discorda

Kodek dźwięku: **Opus**
- Jest najczęściej używanym kodekiem dźwięku w połączeniu z VP9
- Jest to nowoczesny kodek dźwięku pozwalający na uzyskanie większej efektywności od klasycznej MP3 i AAC
- Jest używany m.in. przez Discorda (voice chat), YouTube i wiele innych usług streamingowych

*eksperymentalnie Kodek obrazu: **AV1*** 
- Można go włączyć w zakładce "Zaawansowane"
- Przyszłościowy kodek wideo oferujący najwyższą możliwą efektywność jakości
- Jest używany m.in. na serwisie YouTube

## Kompilacja:
Wymagana aplikacja Microsoft Visual Studio 2019 wraz z odwołaniem axWindowsMediaPlayer.

## Rozwiązywanie problemów:
Q: Aplikacja nie chce wczytać mojego filmu i się zawiesza.
A: Zainstaluj dowolną paczkę kodeków do Windows Media Player. Alternatywnie: Przed zaznaczeniem nagrania kliknij PPM na przycisk "Wybierz nagranie" w oknie aplikacji i zaznacz podaną opcję.

Q: Występuje błąd z enkodowaniem.
A: Sprawdź czy wszystkie ustawienia są poprawne i czy nagranie jest dostępne na dysku. Jeżeli zmieniłeś coś w ścieżkach dźwiękowych, spróbuj ponownie zmienić. Jeżeli to nie pomoże: kliknij PPM na ikonki w zakładce Podsumowanie obok przycisku "Renderuj Nagranie". Tam znajduje się zestaw opcji który pomoże ci znaleźć błąd.

## Prawa Autorskie:
Ikony w aplikacji: https://www.flaticon.com
