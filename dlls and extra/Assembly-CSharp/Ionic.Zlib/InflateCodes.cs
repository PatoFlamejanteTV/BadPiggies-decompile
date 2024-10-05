using System;

namespace Ionic.Zlib;

internal sealed class InflateCodes
{
	private const int START = 0;

	private const int LEN = 1;

	private const int LENEXT = 2;

	private const int DIST = 3;

	private const int DISTEXT = 4;

	private const int COPY = 5;

	private const int LIT = 6;

	private const int WASH = 7;

	private const int END = 8;

	private const int BADCODE = 9;

	internal int mode;

	internal int len;

	internal int[] tree;

	internal int tree_index;

	internal int need;

	internal int lit;

	internal int bitsToGet;

	internal int dist;

	internal byte lbits;

	internal byte dbits;

	internal int[] ltree;

	internal int ltree_index;

	internal int[] dtree;

	internal int dtree_index;

	internal InflateCodes()
	{
	}

	internal void Init(int bl, int bd, int[] tl, int tl_index, int[] td, int td_index)
	{
		mode = 0;
		lbits = (byte)bl;
		dbits = (byte)bd;
		ltree = tl;
		ltree_index = tl_index;
		dtree = td;
		dtree_index = td_index;
		tree = null;
	}

	internal int Process(InflateBlocks blocks, int r)
	{
		ZlibCodec codec = blocks._codec;
		int num = codec.NextIn;
		int num2 = codec.AvailableBytesIn;
		int num3 = blocks.bitb;
		int i = blocks.bitk;
		int num4 = blocks.writeAt;
		int num5 = ((num4 >= blocks.readAt) ? (blocks.end - num4) : (blocks.readAt - num4 - 1));
		while (true)
		{
			switch (mode)
			{
			case 0:
				if (num5 >= 258 && num2 >= 10)
				{
					blocks.bitb = num3;
					blocks.bitk = i;
					codec.AvailableBytesIn = num2;
					codec.TotalBytesIn += num - codec.NextIn;
					codec.NextIn = num;
					blocks.writeAt = num4;
					r = InflateFast(lbits, dbits, ltree, ltree_index, dtree, dtree_index, blocks, codec);
					num = codec.NextIn;
					num2 = codec.AvailableBytesIn;
					num3 = blocks.bitb;
					i = blocks.bitk;
					num4 = blocks.writeAt;
					num5 = ((num4 >= blocks.readAt) ? (blocks.end - num4) : (blocks.readAt - num4 - 1));
					if (r != 0)
					{
						mode = ((r != 1) ? 9 : 7);
						break;
					}
				}
				need = lbits;
				tree = ltree;
				tree_index = ltree_index;
				mode = 1;
				goto case 1;
			case 2:
			{
				int num6;
				for (num6 = bitsToGet; i < num6; i += 8)
				{
					if (num2 != 0)
					{
						r = 0;
						num2--;
						num3 |= (codec.InputBuffer[num++] & 0xFF) << i;
						continue;
					}
					blocks.bitb = num3;
					blocks.bitk = i;
					codec.AvailableBytesIn = num2;
					codec.TotalBytesIn += num - codec.NextIn;
					codec.NextIn = num;
					blocks.writeAt = num4;
					return blocks.Flush(r);
				}
				len += num3 & InternalInflateConstants.InflateMask[num6];
				num3 >>= num6;
				i -= num6;
				need = dbits;
				tree = dtree;
				tree_index = dtree_index;
				mode = 3;
				goto case 3;
			}
			case 4:
			{
				int num6;
				for (num6 = bitsToGet; i < num6; i += 8)
				{
					if (num2 != 0)
					{
						r = 0;
						num2--;
						num3 |= (codec.InputBuffer[num++] & 0xFF) << i;
						continue;
					}
					blocks.bitb = num3;
					blocks.bitk = i;
					codec.AvailableBytesIn = num2;
					codec.TotalBytesIn += num - codec.NextIn;
					codec.NextIn = num;
					blocks.writeAt = num4;
					return blocks.Flush(r);
				}
				dist += num3 & InternalInflateConstants.InflateMask[num6];
				num3 >>= num6;
				i -= num6;
				mode = 5;
				goto case 5;
			}
			case 6:
				if (num5 == 0)
				{
					if (num4 == blocks.end && blocks.readAt != 0)
					{
						num4 = 0;
						num5 = ((num4 >= blocks.readAt) ? (blocks.end - num4) : (blocks.readAt - num4 - 1));
					}
					if (num5 == 0)
					{
						blocks.writeAt = num4;
						r = blocks.Flush(r);
						num4 = blocks.writeAt;
						num5 = ((num4 >= blocks.readAt) ? (blocks.end - num4) : (blocks.readAt - num4 - 1));
						if (num4 == blocks.end && blocks.readAt != 0)
						{
							num4 = 0;
							num5 = ((num4 >= blocks.readAt) ? (blocks.end - num4) : (blocks.readAt - num4 - 1));
						}
						if (num5 == 0)
						{
							blocks.bitb = num3;
							blocks.bitk = i;
							codec.AvailableBytesIn = num2;
							codec.TotalBytesIn += num - codec.NextIn;
							codec.NextIn = num;
							blocks.writeAt = num4;
							return blocks.Flush(r);
						}
					}
				}
				r = 0;
				blocks.window[num4++] = (byte)lit;
				num5--;
				mode = 0;
				break;
			case 1:
			{
				int num6;
				for (num6 = need; i < num6; i += 8)
				{
					if (num2 != 0)
					{
						r = 0;
						num2--;
						num3 |= (codec.InputBuffer[num++] & 0xFF) << i;
						continue;
					}
					blocks.bitb = num3;
					blocks.bitk = i;
					codec.AvailableBytesIn = num2;
					codec.TotalBytesIn += num - codec.NextIn;
					codec.NextIn = num;
					blocks.writeAt = num4;
					return blocks.Flush(r);
				}
				int num7 = (tree_index + (num3 & InternalInflateConstants.InflateMask[num6])) * 3;
				num3 >>= tree[num7 + 1];
				i -= tree[num7 + 1];
				int num8 = tree[num7];
				if (num8 == 0)
				{
					lit = tree[num7 + 2];
					mode = 6;
					break;
				}
				if (((uint)num8 & 0x10u) != 0)
				{
					bitsToGet = num8 & 0xF;
					len = tree[num7 + 2];
					mode = 2;
					break;
				}
				if ((num8 & 0x40) == 0)
				{
					need = num8;
					tree_index = num7 / 3 + tree[num7 + 2];
					break;
				}
				if (((uint)num8 & 0x20u) != 0)
				{
					mode = 7;
					break;
				}
				mode = 9;
				codec.Message = "invalid literal/length code";
				r = -3;
				blocks.bitb = num3;
				blocks.bitk = i;
				codec.AvailableBytesIn = num2;
				codec.TotalBytesIn += num - codec.NextIn;
				codec.NextIn = num;
				blocks.writeAt = num4;
				return blocks.Flush(r);
			}
			case 3:
			{
				int num6;
				for (num6 = need; i < num6; i += 8)
				{
					if (num2 != 0)
					{
						r = 0;
						num2--;
						num3 |= (codec.InputBuffer[num++] & 0xFF) << i;
						continue;
					}
					blocks.bitb = num3;
					blocks.bitk = i;
					codec.AvailableBytesIn = num2;
					codec.TotalBytesIn += num - codec.NextIn;
					codec.NextIn = num;
					blocks.writeAt = num4;
					return blocks.Flush(r);
				}
				int num7 = (tree_index + (num3 & InternalInflateConstants.InflateMask[num6])) * 3;
				num3 >>= tree[num7 + 1];
				i -= tree[num7 + 1];
				int num8 = tree[num7];
				if (((uint)num8 & 0x10u) != 0)
				{
					bitsToGet = num8 & 0xF;
					dist = tree[num7 + 2];
					mode = 4;
					break;
				}
				if ((num8 & 0x40) == 0)
				{
					need = num8;
					tree_index = num7 / 3 + tree[num7 + 2];
					break;
				}
				mode = 9;
				codec.Message = "invalid distance code";
				r = -3;
				blocks.bitb = num3;
				blocks.bitk = i;
				codec.AvailableBytesIn = num2;
				codec.TotalBytesIn += num - codec.NextIn;
				codec.NextIn = num;
				blocks.writeAt = num4;
				return blocks.Flush(r);
			}
			case 5:
			{
				int j;
				for (j = num4 - dist; j < 0; j += blocks.end)
				{
				}
				while (len != 0)
				{
					if (num5 == 0)
					{
						if (num4 == blocks.end && blocks.readAt != 0)
						{
							num4 = 0;
							num5 = ((num4 >= blocks.readAt) ? (blocks.end - num4) : (blocks.readAt - num4 - 1));
						}
						if (num5 == 0)
						{
							blocks.writeAt = num4;
							r = blocks.Flush(r);
							num4 = blocks.writeAt;
							num5 = ((num4 >= blocks.readAt) ? (blocks.end - num4) : (blocks.readAt - num4 - 1));
							if (num4 == blocks.end && blocks.readAt != 0)
							{
								num4 = 0;
								num5 = ((num4 >= blocks.readAt) ? (blocks.end - num4) : (blocks.readAt - num4 - 1));
							}
							if (num5 == 0)
							{
								blocks.bitb = num3;
								blocks.bitk = i;
								codec.AvailableBytesIn = num2;
								codec.TotalBytesIn += num - codec.NextIn;
								codec.NextIn = num;
								blocks.writeAt = num4;
								return blocks.Flush(r);
							}
						}
					}
					blocks.window[num4++] = blocks.window[j++];
					num5--;
					if (j == blocks.end)
					{
						j = 0;
					}
					len--;
				}
				mode = 0;
				break;
			}
			default:
				r = -2;
				blocks.bitb = num3;
				blocks.bitk = i;
				codec.AvailableBytesIn = num2;
				codec.TotalBytesIn += num - codec.NextIn;
				codec.NextIn = num;
				blocks.writeAt = num4;
				return blocks.Flush(r);
			case 7:
				if (i > 7)
				{
					i -= 8;
					num2++;
					num--;
				}
				blocks.writeAt = num4;
				r = blocks.Flush(r);
				num4 = blocks.writeAt;
				if (num4 < blocks.readAt)
				{
					_ = blocks.readAt;
				}
				else
				{
					_ = blocks.end;
				}
				if (blocks.readAt != blocks.writeAt)
				{
					blocks.bitb = num3;
					blocks.bitk = i;
					codec.AvailableBytesIn = num2;
					codec.TotalBytesIn += num - codec.NextIn;
					codec.NextIn = num;
					blocks.writeAt = num4;
					return blocks.Flush(r);
				}
				mode = 8;
				goto case 8;
			case 8:
				r = 1;
				blocks.bitb = num3;
				blocks.bitk = i;
				codec.AvailableBytesIn = num2;
				codec.TotalBytesIn += num - codec.NextIn;
				codec.NextIn = num;
				blocks.writeAt = num4;
				return blocks.Flush(r);
			case 9:
				r = -3;
				blocks.bitb = num3;
				blocks.bitk = i;
				codec.AvailableBytesIn = num2;
				codec.TotalBytesIn += num - codec.NextIn;
				codec.NextIn = num;
				blocks.writeAt = num4;
				return blocks.Flush(r);
			}
		}
	}

	internal int InflateFast(int bl, int bd, int[] tl, int tl_index, int[] td, int td_index, InflateBlocks s, ZlibCodec z)
	{
		int nextIn = z.NextIn;
		int num = z.AvailableBytesIn;
		int num2 = s.bitb;
		int num3 = s.bitk;
		int num4 = s.writeAt;
		int num5 = ((num4 >= s.readAt) ? (s.end - num4) : (s.readAt - num4 - 1));
		int num6 = InternalInflateConstants.InflateMask[bl];
		int num7 = InternalInflateConstants.InflateMask[bd];
		int num11;
		while (true)
		{
			if (num3 < 20)
			{
				num--;
				num2 |= (z.InputBuffer[nextIn++] & 0xFF) << num3;
				num3 += 8;
				continue;
			}
			int num8 = num2 & num6;
			int num9 = (tl_index + num8) * 3;
			int num10;
			if ((num10 = tl[num9]) == 0)
			{
				num2 >>= tl[num9 + 1];
				num3 -= tl[num9 + 1];
				s.window[num4++] = (byte)tl[num9 + 2];
				num5--;
			}
			else
			{
				while (true)
				{
					num2 >>= tl[num9 + 1];
					num3 -= tl[num9 + 1];
					if ((num10 & 0x10) == 0)
					{
						if ((num10 & 0x40) == 0)
						{
							num8 += tl[num9 + 2];
							num8 += num2 & InternalInflateConstants.InflateMask[num10];
							num9 = (tl_index + num8) * 3;
							if ((num10 = tl[num9]) == 0)
							{
								num2 >>= tl[num9 + 1];
								num3 -= tl[num9 + 1];
								s.window[num4++] = (byte)tl[num9 + 2];
								num5--;
								break;
							}
							continue;
						}
						if (((uint)num10 & 0x20u) != 0)
						{
							num11 = z.AvailableBytesIn - num;
							num11 = ((num3 >> 3 >= num11) ? num11 : (num3 >> 3));
							num += num11;
							nextIn -= num11;
							num3 -= num11 << 3;
							s.bitb = num2;
							s.bitk = num3;
							z.AvailableBytesIn = num;
							z.TotalBytesIn += nextIn - z.NextIn;
							z.NextIn = nextIn;
							s.writeAt = num4;
							return 1;
						}
						z.Message = "invalid literal/length code";
						num11 = z.AvailableBytesIn - num;
						num11 = ((num3 >> 3 >= num11) ? num11 : (num3 >> 3));
						num += num11;
						nextIn -= num11;
						num3 -= num11 << 3;
						s.bitb = num2;
						s.bitk = num3;
						z.AvailableBytesIn = num;
						z.TotalBytesIn += nextIn - z.NextIn;
						z.NextIn = nextIn;
						s.writeAt = num4;
						return -3;
					}
					num10 &= 0xF;
					num11 = tl[num9 + 2] + (num2 & InternalInflateConstants.InflateMask[num10]);
					num2 >>= num10;
					for (num3 -= num10; num3 < 15; num3 += 8)
					{
						num--;
						num2 |= (z.InputBuffer[nextIn++] & 0xFF) << num3;
					}
					num8 = num2 & num7;
					num9 = (td_index + num8) * 3;
					num10 = td[num9];
					while (true)
					{
						num2 >>= td[num9 + 1];
						num3 -= td[num9 + 1];
						if (((uint)num10 & 0x10u) != 0)
						{
							break;
						}
						if ((num10 & 0x40) == 0)
						{
							num8 += td[num9 + 2];
							num8 += num2 & InternalInflateConstants.InflateMask[num10];
							num9 = (td_index + num8) * 3;
							num10 = td[num9];
							continue;
						}
						z.Message = "invalid distance code";
						num11 = z.AvailableBytesIn - num;
						num11 = ((num3 >> 3 >= num11) ? num11 : (num3 >> 3));
						num += num11;
						nextIn -= num11;
						num3 -= num11 << 3;
						s.bitb = num2;
						s.bitk = num3;
						z.AvailableBytesIn = num;
						z.TotalBytesIn += nextIn - z.NextIn;
						z.NextIn = nextIn;
						s.writeAt = num4;
						return -3;
					}
					for (num10 &= 0xF; num3 < num10; num3 += 8)
					{
						num--;
						num2 |= (z.InputBuffer[nextIn++] & 0xFF) << num3;
					}
					int num12 = td[num9 + 2] + (num2 & InternalInflateConstants.InflateMask[num10]);
					num2 >>= num10;
					num3 -= num10;
					num5 -= num11;
					int num13;
					if (num4 >= num12)
					{
						num13 = num4 - num12;
						if (num4 - num13 > 0 && 2 > num4 - num13)
						{
							s.window[num4++] = s.window[num13++];
							s.window[num4++] = s.window[num13++];
							num11 -= 2;
						}
						else
						{
							Array.Copy(s.window, num13, s.window, num4, 2);
							num4 += 2;
							num13 += 2;
							num11 -= 2;
						}
					}
					else
					{
						num13 = num4 - num12;
						do
						{
							num13 += s.end;
						}
						while (num13 < 0);
						num10 = s.end - num13;
						if (num11 > num10)
						{
							num11 -= num10;
							if (num4 - num13 > 0 && num10 > num4 - num13)
							{
								do
								{
									s.window[num4++] = s.window[num13++];
								}
								while (--num10 != 0);
							}
							else
							{
								Array.Copy(s.window, num13, s.window, num4, num10);
								num4 += num10;
								num13 += num10;
							}
							num13 = 0;
						}
					}
					if (num4 - num13 > 0 && num11 > num4 - num13)
					{
						do
						{
							s.window[num4++] = s.window[num13++];
						}
						while (--num11 != 0);
					}
					else
					{
						Array.Copy(s.window, num13, s.window, num4, num11);
						num4 += num11;
						num13 += num11;
					}
					break;
				}
			}
			if (num5 < 258 || num < 10)
			{
				break;
			}
		}
		num11 = z.AvailableBytesIn - num;
		num11 = ((num3 >> 3 >= num11) ? num11 : (num3 >> 3));
		num += num11;
		nextIn -= num11;
		num3 -= num11 << 3;
		s.bitb = num2;
		s.bitk = num3;
		z.AvailableBytesIn = num;
		z.TotalBytesIn += nextIn - z.NextIn;
		z.NextIn = nextIn;
		s.writeAt = num4;
		return 0;
	}
}
