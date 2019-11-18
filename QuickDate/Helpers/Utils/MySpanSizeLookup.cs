using Android.Support.V7.Widget;

namespace QuickDate.Helpers.Utils
{
    public class MySpanSizeLookup : GridLayoutManager.SpanSizeLookup
    {
        public int SpanPos, SpanCnt1, SpanCnt2;
        public MySpanSizeLookup(int spanPos, int spanCnt1, int spanCnt2)
        {
            SpanPos = spanPos;
            SpanCnt1 = spanCnt1;
            SpanCnt2 = spanCnt2;
        }

        public override int GetSpanSize(int position)
        {

            return position % SpanPos == 0 ? SpanCnt2 : SpanCnt1;
        }
    }
}