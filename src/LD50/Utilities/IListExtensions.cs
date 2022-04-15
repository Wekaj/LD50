using System;
using System.Collections.Generic;

namespace LD50.Utilities {
    public static class IListExtensions {
        public static void RemoveWhere<T>(this IList<T> list, Predicate<T> predicate) {
            for (int i = 0; i < list.Count; i++) {
                if (!predicate(list[i])) {
                    continue;
                }

                list.RemoveAt(i);
                i--;
            }
        }
    }
}
