// -------------------------------------------------------
// Copyright (C) 施维串 版权所有。
// 创建标识：2013-11-11 10:53:36 Created by 施维串
// 功能说明：
// 注意事项：
// 
// 更新记录：
// -------------------------------------------------------


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Text.RegularExpressions;

namespace shiwch.util
{
    public class TrieNode
    {
        protected internal bool m_end;
        protected internal Dictionary<char, TrieNode> m_values;
        protected internal TrieNode()
        {
            m_values = new Dictionary<char, TrieNode>();
        }

        protected internal bool TryGetValue(char c, out TrieNode node)
        {
            return m_values.TryGetValue(c, out node);
        }

        protected internal TrieNode Add(char c)
        {
            TrieNode subnode;
            if (!m_values.TryGetValue(c, out subnode))
            {
                subnode = new TrieNode();
                m_values.Add(c, subnode);
            }
            return subnode;
        }
    }
    
    /// <summary>
    /// 敏感词过滤算法
    /// </summary>
    public class SensitiveWordParse : TrieNode
    {
        /// <summary>
        /// 添加关键词
        /// </summary>
        /// <param name="key">关键词</param>
        public void AddKey(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return;
            }

            TrieNode node = this;
            for (int i = 0; i < key.Length; i++)
            {
                char c = ToFormal(key[i]);
                if (IsNoisy(c)) continue;
                node = node.Add(c);
            }
            node.m_end = true;
        }

        /// <summary>
        /// 检查是否包含非法字符
        /// </summary>
        public bool HasBadWord(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return false;

            for (int head = 0; head < text.Length; head++)
            {
                int index = head;
                var nextchar = ToFormal(text[index]);
                if (IsNoisy(nextchar)) continue;
                TrieNode node = this;

                while (true)
                {
                    if (!node.TryGetValue(nextchar, out node)) break;

                    if (node.m_end)
                    {
                        return true;
                    }
                    if (text.Length == ++index)
                    {
                        break;
                    }
                    nextchar = ToFormal(text[index]);
                    while (IsNoisy(nextchar))
                    {
                        if (text.Length == ++index)
                        {
                            break;
                        }
                        nextchar = ToFormal(text[index]);
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 替换非法字符
        /// </summary>
        public string ReplaceBadWord(string text, char mask = '*')
        {
            if (string.IsNullOrWhiteSpace(text)) return text;

            char[] chars = null;
            for (int head = 0; head < text.Length; head++)
            {
                int index = head;
                var nextchar = ToFormal(text[index]);
                if (IsNoisy(nextchar)) continue;
                TrieNode node = this;

                while (true)
                {
                    if (!node.TryGetValue(nextchar, out node)) break;

                    if (node.m_end)
                    {
                        if (chars == null) chars = text.ToArray();
                        for (int i = head; i <= index; i++)
                        {
                            chars[i] = mask;
                        }
                        head = index;
                    }
                    if (text.Length == ++index)
                    {
                        break;
                    }
                    nextchar = ToFormal(text[index]);
                    while (IsNoisy(nextchar))
                    {
                        if (text.Length == ++index)
                        {
                            break;
                        }
                        nextchar = ToFormal(text[index]);
                    }
                }
            }
            return chars == null ? text : new string(chars);
        }

        static char[] noisyChar = new char[] { ' ', '|', '*', '★' };
        private static bool IsNoisy(char c)
        {
            foreach (var nc in noisyChar)
            {
                if (nc == c) return true;
            }
            return false;
        }
        private static char ToFormal(char c)
        {
            var r = c.GetSimp();
            if (r >= 'A' && r <= 'Z') return (char)(r + 32);
            return r;
        }
    }
}
