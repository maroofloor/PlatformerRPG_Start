public class AllStruct
{
    public struct Stat
    {
        public float HP; // ü��
        public float MaxHP; // �ִ�ü��
        public float Att; // ���ݷ�
        public float Def; // ����        

        public Stat(int hp, int att, int def = 0) // ������ ��ġ�� ���� ������ 0���� ����
        {
            HP = hp;
            MaxHP = hp;
            Att = att;
            Def = def;
        }
    }
}
