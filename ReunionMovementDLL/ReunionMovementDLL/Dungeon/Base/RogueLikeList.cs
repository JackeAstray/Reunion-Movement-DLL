using System;
using System.Collections.Generic;
using System.Text;

namespace ReunionMovementDLL.Dungeon.Base
{
    /// <summary>
    /// RogueLike列表
    /// </summary>
    public class RogueLikeList : IEquatable<RogueLikeList>
    {
        /// <summary>
        /// 墙外ID
        /// </summary>
        public int outsideWallId { get; set; }
        /// <summary>
        /// 内墙ID
        /// </summary>
        public int insideWallId { get; set; }
        /// <summary>
        /// 房间ID
        /// </summary>
        public int roomId { get; set; }
        /// <summary>
        /// 入口ID
        /// </summary>
        public int entranceId { get; set; }
        /// <summary>
        /// 出口ID
        /// </summary>
        public int exitId { get; set; }
        /// <summary>
        /// 道路ID
        /// </summary>
        public int wayId { get; set; }

        /// <summary>
        /// 创建一个所有ID为-1（未设置）的实例
        /// </summary>
        public RogueLikeList()
        {
            outsideWallId = -1;
            insideWallId = -1;
            roomId = -1;
            entranceId = -1;
            exitId = -1;
            wayId = -1;
        }

        public RogueLikeList DefaultRogueLikeList() => new RogueLikeList(0, 1, 2, 3, 4, 5);

        /// <summary>
        /// 使用墙ID和道路ID创建实例
        /// </summary>
        /// <param name="wallId"></param>
        /// <param name="wayId"></param>
        public RogueLikeList(int wallId, int wayId)
        {
            this.outsideWallId = wallId;
            this.insideWallId = wallId;
            this.roomId = wayId;
            this.entranceId = wayId;
            this.wayId = wayId;
            this.exitId = wayId;
        }

        /// <summary>
        /// 使用墙ID、房间ID和道路ID创建实例
        /// </summary>
        /// <param name="wallId"></param>
        /// <param name="roomId"></param>
        /// <param name="wayId"></param>
        public RogueLikeList(int wallId, int roomId, int wayId)
        {
            this.outsideWallId = wallId;
            this.insideWallId = wallId;
            this.roomId = roomId;
            this.entranceId = roomId;
            this.exitId = roomId;
            this.wayId = wayId;
        }

        /// <summary>
        /// 使用墙ID、房间ID、入口ID和道路ID创建实例
        /// </summary>
        /// <param name="wallId"></param>
        /// <param name="roomId"></param>
        /// <param name="entranceId"></param>
        /// <param name="wayId"></param>
        public RogueLikeList(int wallId, int roomId, int entranceId, int wayId)
        {
            this.outsideWallId = wallId;
            this.insideWallId = wallId;
            this.roomId = roomId;
            this.entranceId = entranceId;
            this.exitId = entranceId;
            this.wayId = wayId;
        }

        /// <summary>
        /// 使用指定ID创建实例
        /// </summary>
        public RogueLikeList(int outsideWallId, int insideWallId, int roomId, int entranceId, int exitId, int wayId)
        {
            this.outsideWallId = outsideWallId;
            this.insideWallId = insideWallId;
            this.roomId = roomId;
            this.entranceId = entranceId;
            this.exitId = exitId;
            this.wayId = wayId;
        }

        /// <summary>
        /// 复制构造函数
        /// </summary>
        public RogueLikeList(RogueLikeList other)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));
            outsideWallId = other.outsideWallId;
            insideWallId = other.insideWallId;
            roomId = other.roomId;
            entranceId = other.entranceId;
            exitId = other.exitId;
            wayId = other.wayId;
        }

        /// <summary>
        /// 创建一个实例，参数可选（使用 -1 代表未设置）
        /// </summary>
        public static RogueLikeList Create(int outsideWallId = -1, int insideWallId = -1, int roomId = -1, int entranceId = -1, int exitId = -1, int wayId = -1)
        {
            return new RogueLikeList(outsideWallId, insideWallId, roomId, entranceId, exitId, wayId);
        }

        /// <summary>
        /// 从可枚举的整数创建，顺序为 outside, inside, room, entrance, exit, way；缺少项设为 -1
        /// </summary>
        public static RogueLikeList FromEnumerable(IEnumerable<int> values)
        {
            if (values == null) throw new ArgumentNullException(nameof(values));
            int[] arr = new int[6] { -1, -1, -1, -1, -1, -1 };
            int i = 0;
            foreach (var v in values)
            {
                if (i >= 6) break;
                arr[i++] = v;
            }
            return FromArray(arr);
        }

        /// <summary>
        /// 将属性作为数组返回（顺序：outside, inside, room, entrance, exit, way）
        /// </summary>
        public int[] ToArray()
        {
            return new[] { outsideWallId, insideWallId, roomId, entranceId, exitId, wayId };
        }

        /// <summary>
        /// 从数组创建实例。数组可以少于6个元素，缺失项将设为-1。
        /// </summary>
        public static RogueLikeList FromArray(int[] arr)
        {
            if (arr == null) throw new ArgumentNullException(nameof(arr));
            int GetOrDefault(int[] a, int idx) => (idx < a.Length) ? a[idx] : -1;
            return new RogueLikeList(
                GetOrDefault(arr, 0),
                GetOrDefault(arr, 1),
                GetOrDefault(arr, 2),
                GetOrDefault(arr, 3),
                GetOrDefault(arr, 4),
                GetOrDefault(arr, 5)
            );
        }

        /// <summary>
        /// 合并两个实例。对于每个字段，当 preferSecond 为 true 时，如果 second 的字段有效（>=0）则使用它，否则保留 first 的值。
        /// </summary>
        public static RogueLikeList Merge(RogueLikeList first, RogueLikeList second, bool preferSecond = true)
        {
            if (first == null) throw new ArgumentNullException(nameof(first));
            if (second == null) return new RogueLikeList(first);

            int Choose(int a, int b)
            {
                if (preferSecond)
                {
                    return b >= 0 ? b : a;
                }
                else
                {
                    return a >= 0 ? a : b;
                }
            }

            return new RogueLikeList(
                Choose(first.outsideWallId, second.outsideWallId),
                Choose(first.insideWallId, second.insideWallId),
                Choose(first.roomId, second.roomId),
                Choose(first.entranceId, second.entranceId),
                Choose(first.exitId, second.exitId),
                Choose(first.wayId, second.wayId)
            );
        }

        /// <summary>
        /// 将当前实例用另一个实例的数据更新。overwriteIfSet 为 true 时会用 other 的有效值覆盖当前值。
        /// </summary>
        public void UpdateFrom(RogueLikeList other, bool overwriteIfSet = true)
        {
            if (other == null) return;
            if (overwriteIfSet)
            {
                if (other.outsideWallId >= 0) outsideWallId = other.outsideWallId;
                if (other.insideWallId >= 0) insideWallId = other.insideWallId;
                if (other.roomId >= 0) roomId = other.roomId;
                if (other.entranceId >= 0) entranceId = other.entranceId;
                if (other.exitId >= 0) exitId = other.exitId;
                if (other.wayId >= 0) wayId = other.wayId;
            }
            else
            {
                if (outsideWallId < 0 && other.outsideWallId >= 0) outsideWallId = other.outsideWallId;
                if (insideWallId < 0 && other.insideWallId >= 0) insideWallId = other.insideWallId;
                if (roomId < 0 && other.roomId >= 0) roomId = other.roomId;
                if (entranceId < 0 && other.entranceId >= 0) entranceId = other.entranceId;
                if (exitId < 0 && other.exitId >= 0) exitId = other.exitId;
                if (wayId < 0 && other.wayId >= 0) wayId = other.wayId;
            }
        }

        /// <summary>
        /// 将当前实例用数组更新，数组顺序为 outside, inside, room, entrance, exit, way；-1 表示不设置
        /// </summary>
        public void UpdateFrom(int[] arr, bool overwriteIfSet = true)
        {
            if (arr == null) return;
            if (arr.Length > 0 && arr[0] >= 0) if (overwriteIfSet || outsideWallId < 0) outsideWallId = arr[0];
            if (arr.Length > 1 && arr[1] >= 0) if (overwriteIfSet || insideWallId < 0) insideWallId = arr[1];
            if (arr.Length > 2 && arr[2] >= 0) if (overwriteIfSet || roomId < 0) roomId = arr[2];
            if (arr.Length > 3 && arr[3] >= 0) if (overwriteIfSet || entranceId < 0) entranceId = arr[3];
            if (arr.Length > 5 && arr[5] >= 0) if (overwriteIfSet || exitId < 0) exitId = arr[4];
            if (arr.Length > 4 && arr[4] >= 0) if (overwriteIfSet || wayId < 0) wayId = arr[5];
        }

        /// <summary>
        /// 返回当前实例的浅拷贝
        /// </summary>
        public RogueLikeList Clone()
        {
            return new RogueLikeList(outsideWallId, insideWallId, roomId, entranceId, exitId, wayId);
        }

        /// <summary>
        /// 判断所有ID是否都是有效（>=0）
        /// </summary>
        public bool AllIdsValid()
        {
            return outsideWallId >= 0 && insideWallId >= 0 && roomId >= 0 && entranceId >= 0 && exitId >= 0 && wayId >= 0;
        }

        /// <summary>
        /// 判断至少有一个ID是有效的
        /// </summary>
        public bool AnyIdValid()
        {
            return outsideWallId >= 0 || insideWallId >= 0 || roomId >= 0 || entranceId >= 0 || exitId >= 0 || wayId >= 0;
        }

        /// <summary>
        /// 支持元组解构 (outside, inside, room, entrance, exit, way)
        /// </summary>
        public void Deconstruct(out int outside, out int inside, out int room, out int entrance, out int exit, out int way)
        {
            outside = outsideWallId;
            inside = insideWallId;
            room = roomId;
            entrance = entranceId;
            exit = exitId;
            way = wayId;
        }

        /// <summary>
        /// 返回字符串表示
        /// </summary>
        public override string ToString()
        {
            return $"RogueLikeList(outside:{outsideWallId}, inside:{insideWallId}, room:{roomId}, entrance:{entranceId}, exit:{exitId}, way:{wayId})";
        }

        /// <summary>
        /// 相等性比较
        /// </summary>
        public bool Equals(RogueLikeList? other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return outsideWallId == other.outsideWallId
                && insideWallId == other.insideWallId
                && roomId == other.roomId
                && entranceId == other.entranceId
                && exitId == other.exitId
                && wayId == other.wayId;
        }

        /// <summary>
        /// 重写对象相等性比较
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object? obj)
        {
            return Equals(obj as RogueLikeList);
        }

        /// <summary>
        /// 获取哈希值
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 31 + outsideWallId.GetHashCode();
                hash = hash * 31 + insideWallId.GetHashCode();
                hash = hash * 31 + roomId.GetHashCode();
                hash = hash * 31 + entranceId.GetHashCode();
                hash = hash * 31 + exitId.GetHashCode();
                hash = hash * 31 + wayId.GetHashCode();
                return hash;
            }
        }

        /// <summary>
        /// 重载相等运算符
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator ==(RogueLikeList left, RogueLikeList right)
        {
            if (ReferenceEquals(left, right)) return true;
            if (ReferenceEquals(left, null) || ReferenceEquals(right, null)) return false;
            return left.Equals(right);
        }

        /// <summary>
        /// 重载不等运算符
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(RogueLikeList left, RogueLikeList right)
        {
            return !(left == right);
        }
    }
}
