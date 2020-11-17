# 코드
## Xml에서 데이터 가져오기
<pre><code>
IEnumerator CreateNotes(ulong startTime){
		Debug.Log ("Start create notes. BGM type is " + songType + ".");
		AudioStackData asd = new AudioStackData(GameData._data.ad[songType]._audio, (long)lastSongTime);//다음 곡 생성
		audioStack.Insert(audioStack.Count, asd);//다음 곡 스택에 쌓아놓기
		createNoteTime = lastSongTime;//마지막 노트가 생성된 시간을 노래 끝 부분으로 initial
		lastSongTime += (ulong)GameData._data.ad[songType].time;//이번 노래의 끝 시점을 저장
		TextAsset ta = Resources.Load("Data/NoteData" + songType.ToString ()) as TextAsset;//Text로 되어 있는 데이터 불러오기
		MemoryStream ms = new MemoryStream(ta.bytes);//데이터 메모리에 안주
		XmlTextReader xtr = new XmlTextReader(ms);//메모리에 올린 데이터를 Xml 형태로 읽어들임
		XmlDocument xml = new XmlDocument();//Xml문서 구조를 읽어내기 위해 XmlDocument클래스 객체 생성
		xml.Load(xtr);//데이터 불러오기
		XmlNodeList notes = xml.SelectSingleNode("/NoteData").ChildNodes;//각 데이터들을 배열로 불러옴
		int count = 0;
		foreach(XmlNode n in notes){//각 데이터 객체화 시작
			ulong timing = ulong.Parse(n.Attributes["time"].Value);//터치라인에 노트가 도착할 타이밍 불러오기
			if(n.Attributes["note1"].Value != "NULL"){
				int type = int.Parse(n.Attributes["note1"].Value);//노트 타입 불러오기
				CreateNote (startTime, 0, timing, type);//위에서 불러온 데이터로 노트 생성
			}
			if(n.Attributes["note2"].Value != "NULL"){
				int type = int.Parse(n.Attributes["note2"].Value);
				CreateNote (startTime, 1, timing, type);
			}
			if(n.Attributes["note3"].Value != "NULL"){
				int type = int.Parse(n.Attributes["note3"].Value);
				CreateNote (startTime, 2, timing, type);
			}
			if(n.Attributes["note4"].Value != "NULL"){
				int type = int.Parse(n.Attributes["note4"].Value);
				CreateNote (startTime, 3, timing, type);
			}
			if(n.Attributes["note5"].Value != "NULL"){
				int type = int.Parse(n.Attributes["note5"].Value);
				CreateNote (startTime, 4, timing, type);
			}
			if(n.Attributes["note6"].Value != "NULL"){
				int type = int.Parse(n.Attributes["note6"].Value);
				CreateNote (startTime, 5, timing, type);
			}
			if(n.Attributes["note7"].Value != "NULL"){
				int type = int.Parse(n.Attributes["note7"].Value);
				CreateNote (startTime, 6, timing, type);
			}
			if(n.Attributes["note8"].Value != "NULL"){
				int type = int.Parse(n.Attributes["note8"].Value);
				CreateNote (startTime, 7, timing, type);
			}
			count++;
			if(count > 2){//게임 도중 딜레이를 없애기 위해 노트를 2개이상 생성했을 경우 다음 프레임에 계속해서 작업하도록 함
				yield return null;
				count = 0;
			}
		}
		xtr.Close ();//데이터를 모두 읽어 들였으므로 객체 소멸
		ms.Close();//데이터를 안주시켰던 메모리도 더이상 필요 없으므로 소멸
		Debug.Log ("Done creating notes");
		yield break;//코루틴을 끝냈다
	}
</code></pre>
