public class AllStruct
{
    public struct Stat
    {
        public float HP; // ü��
        public float MaxHP; // �ִ�ü��
        public float Att; // ���ݷ�
        public float Def; // ����        

        public Stat(float hp, float att, float def = 0f) // ������ ��ġ�� ���� ������ 0���� ����
        {
            HP = hp;
            MaxHP = hp;
            Att = att;
            Def = def;
        }
    }
}
