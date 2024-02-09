# Socket
--- 4일차 --- <br> 
패킷 직렬화 작업 <br>
![image](https://github.com/swyou1123/Socket/assets/98148597/7ba6d4d3-ba52-4898-93d9-17c29320e0f2) <br>
패킷에 대한 하드 코딩 되어 있던 작업을 <br>
보내는 것과 받는 것에 대한 자동화 및 모듈화를 진행
<br><br> 

첫번쨰로 간단하게 Session페이지를 추가해주고 코드를 분리 시킨후 <br>  
들어올 수 있는 대부분의 데이터 형을 변환하고 패킷에 넣게 되는 구조부터 파악하자 <br><br>

# 입력부분
Type 1. 정수형 or 실수형 <br>
간단하다 <br><br>![image](https://github.com/swyou1123/Socket/assets/98148597/5fc87f14-4d6c-45de-b474-393e1f5593cb) &nbsp;&nbsp; 플레이어의 정보를 보낼 패킷 클래스를 정의!! <br><br>
![image](https://github.com/swyou1123/Socket/assets/98148597/d20cb37b-6fde-48ce-a74a-05931d58be8c)  &nbsp;&nbsp; 어떤 타입의 패킷인지 emum 타입으로 정의 <br><br>
  


![image](https://github.com/swyou1123/Socket/assets/98148597/00176bb6-4e93-4b2c-a0e9-afa7880f62d0)  &nbsp;&nbsp; 현재 접속할떄 패킷을 보내는 로직에서 해당 그림과 같이 플레이어 아이디 입력한다는 가정 <br><br>


 ![image](https://github.com/swyou1123/Socket/assets/98148597/ec828d35-3ad1-4312-abf2-87854ac83cb5)<br>
  Write 함수를 살펴보면 ChunkSize 크기의 버퍼을 열어두고 총 사이즈에 대한 크기를 먼저 띄워논 후 packetId 와 playerId 를 순서대로 입력한다. <br>
  추후 마지막에 총 사이즈(count) 를 넣어주고 사이즈 만큼의 버퍼를 닫아주고 리턴한다. <br><br>


Type2. 문자열<br>
문자열은 정해저있는 크기가 없다. 그렇기에 문자열 의 경우 문자열의 패킷 앞에 문자열 길이 데이터를 먼저 넣어주고 해당 길이로 문자열을 읽는 시스템을 구현하자<br>
![image](https://github.com/swyou1123/Socket/assets/98148597/2eecf1c4-1549-43a3-9f4b-c3ab23040ed4) <br>
문자열의 길이를 넣어둘 공간을 추가해서 This.name 을 넣어주고 이후 nameLen 을 넣어주는 방식으로 구현<br><br>

Type3. 리스트<br>
스킬에 대한 정보를 여러개 보내야 하는 경우가 있을 수 있다. 구조체 관련으로 보자.<br>
![image](https://github.com/swyou1123/Socket/assets/98148597/de27779f-1acc-41f5-a163-da997a5d5e6b) 구조체 내에서 Write 함수를 따로 구현한다.<br>

![image](https://github.com/swyou1123/Socket/assets/98148597/31ffb748-3149-470e-89f5-3924cbf3e03e) 먼져 스킬에 대한 총 갯수를 입력해주고 <br>
각각의 스킬을 입력해 주면서 사이즈(count)를 올려준다<br><br>

Write 전체코드

```
public override ArraySegment<byte> Write()
{
    ArraySegment<byte> segment = SendBufferHelper.Open(4096);

    ushort count = 0;
    bool success = true;

    Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

    count += sizeof(ushort);
    success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.packetId);
    count += sizeof(ushort);
    success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.playerId);
    count += sizeof(long);

    // string
    ushort nameLen = (ushort)Encoding.Unicode.GetBytes(this.name, 0, this.name.Length, segment.Array, segment.Offset + count + sizeof(ushort));
    success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), nameLen);
    count += sizeof(ushort);
    count += nameLen;

    // list
    success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort) skills.Count);
    count += sizeof(ushort);
    foreach(SkillInfo skill in skills)
        success &= skill.Write(s, ref count);

    success &= BitConverter.TryWriteBytes(s, count);

    if (success == false)
        return null;

    return SendBufferHelper.Close(count);
}
```

Read 전체코드
```
public override void Read(ArraySegment<byte> segment)
{
    ushort count = 0;

    ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);

    count += sizeof(ushort);
    count += sizeof(ushort);
    this.playerId = BitConverter.ToInt64(s.Slice(count, s.Length - count));
    count += sizeof(long);

    // string
    ushort nameLen = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
    count += sizeof(ushort);
    this.name = Encoding.Unicode.GetString(s.Slice(count, nameLen));
    count += nameLen;


    // list
    ushort skillLen = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
    count += sizeof(ushort);
    skills.Clear();
    for (int i = 0; i < skillLen; i++)
    {
        SkillInfo skill = new SkillInfo();
        skill.Read(s, ref count);
        skills.Add(skill);
    }
}
```

패킷 직렬화 작업을 진행했지만 자동화 작업을 통해서 어떠한 데이터가 올지 어떻게 처리해야할지 자동화 처리 부분이 남아있다.


  

