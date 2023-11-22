public class AllStruct
{
    public struct Stat
    {
        public float HP; // 체력
        public float MaxHP; // 최대체력
        public float Att; // 공격력
        public float Def; // 방어력        

        public Stat(float hp, float att, float def = 0f) // 방어력은 수치를 넣지 않으면 0으로 설정
        {
            HP = hp;
            MaxHP = hp;
            Att = att;
            Def = def;
        }
    }
}
