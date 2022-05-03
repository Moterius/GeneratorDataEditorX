﻿/*
 * 由SharpDevelop创建。
 * 用户： Acer
 * 日期: 5月18 星期日
 * 时间: 17:01
 * 
 */
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Text;

namespace DataEditorX.Core
{
    /// <summary>
    /// SQLite 操作
    /// </summary>
    public static class DataBase
    {
        #region 默认
        static readonly string _defaultSQL;
        static readonly string _defaultTableSQL;
        static readonly string _defaultOTableSQL;

        static DataBase()
        {
            _defaultSQL =
                "SELECT datas.*,texts.* FROM datas,texts WHERE datas.id=texts.id ";
            StringBuilder st = new StringBuilder();
            st.Append(@"CREATE TABLE texts(id integer primary key,name text,desc text");
            for (int i = 1; i <= 16; i++)
            {
                st.Append(",str");
                st.Append(i.ToString());
                st.Append(" text");
            }
            st.Append(");");
            st.Append(@"CREATE TABLE datas(");
            st.Append("id integer primary key,ot integer,alias integer,");
            st.Append("setcode integer,type integer,atk integer,def integer,");
            st.Append("level integer,race integer,attribute integer,category integer) ");
            _defaultTableSQL = st.ToString();
            st.Remove(0, st.Length);
            StringBuilder ost = new StringBuilder();
            ost.Append(@"CREATE TABLE texts(id integer primary key,name text,desc text");
            for (int i = 1; i <= 16; i++)
            {
                ost.Append(",str");
                ost.Append(i.ToString());
                ost.Append(" text");
            }
            ost.Append(");");
            ost.Append(@"CREATE TABLE banlists (id integer unique primary key, name text);");
            ost.Append(@"CREATE TABLE bandatas (id integer default 1,flag integer default 0,
            banlistid integer default 0,flagtype integer default 0,limits integer default 0,
            mode integer default 0,location integer default 0);");
            ost.Append(@"CREATE TABLE datas(id integer primary key default 0,ot integer default 0,
            alias integer default 0,setcode integer default 0,type integer default 0,atk integer default 0,
            def integer default 0,level integer default 0,race integer default 0,attribute integer default 0,
            category integer default 0,genre integer default 0, script blob,support integer default 0,
            ocgdate integer default 253402207200,tcgdate integer default 253402207200);");
            _defaultOTableSQL = ost.ToString();
            ost.Remove(0, ost.Length);
        }
        #endregion

        #region 创建数据库
        /// <summary>
        /// 创建数据库
        /// </summary>
        /// <param name="Db">新数据库路径</param>
        public static bool Create(string Db)
        {
            if (File.Exists(Db))
            {
                File.Delete(Db);
            }

            try
            {
                SQLiteConnection.CreateFile(Db);
                if (Db.EndsWith(".db", StringComparison.OrdinalIgnoreCase) || Db.EndsWith(".bytes", StringComparison.OrdinalIgnoreCase)) Command(Db, _defaultOTableSQL);
                else Command(Db, _defaultTableSQL);
            }
            catch
            {
                return false;
            }
            return true;
        }
        public static bool CheckTable(string db)
        {
            try
            {
                if (db.EndsWith(".db", StringComparison.OrdinalIgnoreCase) || db.EndsWith(".bytes", StringComparison.OrdinalIgnoreCase)) Command(db, _defaultOTableSQL);
                else Command(db, _defaultTableSQL);
            }
            catch
            {
                return false;
            }
            return true;
        }
        #endregion

        #region 执行sql语句
        /// <summary>
        /// 执行sql语句
        /// </summary>
        /// <param name="DB">数据库</param>
        /// <param name="SQLs">sql语句</param>
        /// <returns>返回影响行数</returns>
        public static int Command(string DB, params string[] SQLs)
        {
            int result = 0;
            if (File.Exists(DB) && SQLs != null)
            {
                using (SQLiteConnection con = new SQLiteConnection(@"Data Source=" + DB))
                {
                    con.Open();
                    using (SQLiteTransaction trans = con.BeginTransaction())
                    {
                        try
                        {
                            using (SQLiteCommand cmd = new SQLiteCommand(con))
                            {
                                foreach (string SQLstr in SQLs)
                                {
                                    cmd.CommandText = SQLstr;
                                    result += cmd.ExecuteNonQuery();
                                }
                            }
                        }
                        catch
                        {
                            trans.Rollback();//出错，回滚
                            result = -1;
                        }
                        finally
                        {
                            trans.Commit();
                        }
                    }
                    con.Close();
                }
            }
            return result;
        }
        #endregion

        #region 根据SQL读取
        static Card ReadCard(SQLiteDataReader reader, bool reNewLine)
        {
            Card c = new Card(0);
            try
            {
                c.id = reader.GetInt64(reader.GetOrdinal("id"));
                c.ot = reader.GetInt32(reader.GetOrdinal("ot"));
                c.alias = reader.GetInt64(reader.GetOrdinal("alias"));
                c.setcode = reader.GetInt64(reader.GetOrdinal("setcode"));
                c.type = reader.GetInt64(reader.GetOrdinal("type"));
                c.atk = reader.GetInt32(reader.GetOrdinal("atk"));
                c.def = reader.GetInt32(reader.GetOrdinal("def"));
                c.level = reader.GetInt64(reader.GetOrdinal("level"));
                c.race = reader.GetInt64(reader.GetOrdinal("race"));
                c.attribute = reader.GetInt32(reader.GetOrdinal("attribute"));
                c.category = reader.GetInt64(reader.GetOrdinal("genre"));
                c.omega = new long[5];
                c.omega[0] = 1L;
                c.omega[1] = reader.GetInt64(reader.GetOrdinal("category"));
                c.omega[2] = reader.GetInt64(reader.GetOrdinal("support"));
                c.omega[3] = reader.GetInt64(reader.GetOrdinal("ocgdate"));
                c.omega[4] = reader.GetInt64(reader.GetOrdinal("tcgdate"));
                c.name = reader.GetString(reader.GetOrdinal("name"));
                c.desc = reader.GetString(reader.GetOrdinal("desc"));
            }
            catch(Exception e)
            {
                c.id = reader.GetInt64(reader.GetOrdinal("id"));
                c.ot = reader.GetInt32(reader.GetOrdinal("ot"));
                c.alias = reader.GetInt64(reader.GetOrdinal("alias"));
                c.setcode = reader.GetInt64(reader.GetOrdinal("setcode"));
                c.type = reader.GetInt64(reader.GetOrdinal("type"));
                c.atk = reader.GetInt32(reader.GetOrdinal("atk"));
                c.def = reader.GetInt32(reader.GetOrdinal("def"));
                c.level = reader.GetInt64(reader.GetOrdinal("level"));
                c.race = reader.GetInt64(reader.GetOrdinal("race"));
                c.attribute = reader.GetInt32(reader.GetOrdinal("attribute"));
                c.category = reader.GetInt64(reader.GetOrdinal("category"));
                c.omega = new long[5] { 0L, 0L, 0L, 253402207200L, 253402207200L };
                c.name = reader.GetString(reader.GetOrdinal("name"));
                c.desc = reader.GetString(reader.GetOrdinal("desc"));
            }
            if (reNewLine)
            {
                c.desc = Retext(c.desc);
            }

            for (int i = 0; i < 0x10; i++)
            {
                string temp = reader.GetString(reader.GetOrdinal("str" + (i + 1).ToString()));
                c.Str[i] = temp ?? "";
            }
            return c;
        }
        static string Retext(string text)
        {
            StringBuilder sr = new StringBuilder(text);
            sr.Replace("\r\n", "\n");
            sr.Replace("\n", Environment.NewLine);//换为当前系统的换行符
            text = sr.ToString();
            sr.Remove(0, sr.Length);
            return text;
        }

        public static Card[] Read(string DB, bool reNewLine, params long[] ids)
        {
            List<string> idlist = new List<string>();
            foreach (long id in ids)
            {
                idlist.Add(id.ToString());
            }
            return Read(DB, reNewLine, idlist.ToArray());
        }
        /// <summary>
        /// 根据密码集合，读取数据
        /// </summary>
        /// <param name="DB">数据库</param>
        /// <param name="reNewLine">调整换行符</param>
        /// <param name="SQLs">SQL/密码语句集合集合</param>
        public static Card[] Read(string DB, bool reNewLine, params string[] SQLs)
        {
            List<Card> list = new List<Card>();
            List<long> idlist = new List<long>();
            if (File.Exists(DB) && SQLs != null)
            {
                using (SQLiteConnection sqliteconn = new SQLiteConnection(@"Data Source=" + DB))
                {
                    sqliteconn.Open();
                    using (SQLiteTransaction trans = sqliteconn.BeginTransaction())
                    {
                        using (SQLiteCommand sqlitecommand = new SQLiteCommand(sqliteconn))
                        {
                            foreach (string str in SQLs)
                            {
                                int.TryParse(str, out int tmp);

                                string SQLstr;
                                if (string.IsNullOrEmpty(str))
                                {
                                    SQLstr = _defaultSQL;
                                }
                                else if (tmp > 0)
                                {
                                    SQLstr = _defaultSQL + " and datas.id=" + tmp.ToString();
                                }
                                else if (str.StartsWith("select", StringComparison.OrdinalIgnoreCase))
                                {
                                    SQLstr = str;
                                }
                                else if (str.IndexOf("and ") >= 0)
                                {
                                    SQLstr = _defaultSQL + str;
                                }
                                else
                                {
                                    SQLstr = _defaultSQL + " and texts.name like '%" + str + "%'";
                                }

                                sqlitecommand.CommandText = SQLstr;
                                using (SQLiteDataReader reader = sqlitecommand.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        Card c = ReadCard(reader, reNewLine);
                                        if (idlist.IndexOf(c.id) < 0)
                                        {//不存在，则添加
                                            idlist.Add(c.id);
                                            list.Add(c);
                                        }
                                    }
                                    reader.Close();
                                }
                            }
                        }
                        trans.Commit();
                    }
                    sqliteconn.Close();
                }
            }
            if (list.Count == 0)
            {
                return null;
            }

            return list.ToArray();
        }
        #endregion

        #region 复制数据库
        /// <summary>
        /// 复制数据库
        /// </summary>
        /// <param name="DB">复制到的数据库</param>
        /// <param name="cards">卡片集合</param>
        /// <param name="ignore">是否忽略存在</param>
        /// <returns>更新数x2</returns>
        public static int CopyDB(string DB, bool ignore, params Card[] cards)
        {
            int result = 0;
            if (File.Exists(DB) && cards != null)
            {
                using (SQLiteConnection con = new SQLiteConnection(@"Data Source=" + DB))
                {
                    con.Open();
                    using (SQLiteTransaction trans = con.BeginTransaction())
                    {
                        using (SQLiteCommand cmd = new SQLiteCommand(con))
                        {
                            foreach (Card c in cards)
                            {
                                cmd.CommandText = (DB.EndsWith(".db", StringComparison.OrdinalIgnoreCase) || DB.EndsWith(".bytes", StringComparison.OrdinalIgnoreCase)) ? OmegaGetInsertSQL(c, ignore) : GetInsertSQL(c, ignore);
                                result += cmd.ExecuteNonQuery();
                            }
                        }
                        trans.Commit();
                    }
                    con.Close();
                }
            }
            return result;
        }
        #endregion

        #region 删除记录
        public static int DeleteDB(string DB, params Card[] cards)
        {
            int result = 0;
            if (File.Exists(DB) && cards != null)
            {
                using (SQLiteConnection con = new SQLiteConnection(@"Data Source=" + DB))
                {
                    con.Open();
                    using (SQLiteTransaction trans = con.BeginTransaction())
                    {
                        using (SQLiteCommand cmd = new SQLiteCommand(con))
                        {
                            foreach (Card c in cards)
                            {
                                cmd.CommandText = GetDeleteSQL(c);
                                result += cmd.ExecuteNonQuery();
                            }
                        }
                        trans.Commit();
                    }
                    con.Close();
                }
            }
            return result;
        }
        #endregion

        #region 压缩数据库
        public static void Compression(string db)
        {
            if (File.Exists(db))
            {
                using (SQLiteConnection con = new SQLiteConnection(@"Data Source=" + db))
                {
                    con.Open();
                    using (SQLiteCommand cmd = new SQLiteCommand(con))
                    {
                        cmd.CommandText = "vacuum";
                        cmd.ExecuteNonQuery();
                    }
                    con.Close();
                }
            }

        }
        #endregion

        #region SQL语句
        #region 查询
        static string toInt(long l)
        {
            unchecked
            {
                return ((int)l).ToString();
            }
        }
        public static string OmegaGetSelectSQL(Card c)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT datas.*,texts.* FROM datas,texts WHERE datas.id=texts.id ");
            if (c == null)
            {
                return sb.ToString();
            }

            if (!string.IsNullOrEmpty(c.name))
            {
                if (c.name.IndexOf("%%") >= 0)
                {
                    c.name = c.name.Replace("%%", "%");
                }
                else
                {
                    c.name = "%" + c.name.Replace("%", "/%").Replace("_", "/_") + "%";
                }

                sb.Append(" and texts.name like '" + c.name.Replace("'", "''") + "' ");
            }
            if (!string.IsNullOrEmpty(c.desc))
            {
                sb.Append(" and texts.desc like '%" + c.desc.Replace("'", "''") + "%' ");
            }

            if (c.ot > 0)
            {
                sb.Append(" and datas.ot = " + c.ot.ToString());
            }

            if (c.attribute > 0)
            {
                sb.Append(" and datas.attribute = " + c.attribute.ToString());
            }

            if ((c.level & 0xff) > 0)
            {
                sb.Append(" and (datas.level & 255) = " + toInt(c.level & 0xff));
            }

            if ((c.level & 0xff000000) > 0)
            {
                sb.Append(" and (datas.level & 4278190080) = " + toInt(c.level & 0xff000000));
            }

            if ((c.level & 0xff0000) > 0)
            {
                sb.Append(" and (datas.level & 16711680) = " + toInt(c.level & 0xff0000));
            }

            if (c.race > 0)
            {
                sb.Append(" and datas.race = " + toInt(c.race));
            }

            if (c.type > 0)
            {
                sb.Append(" and datas.type & " + toInt(c.type) + " = " + toInt(c.type));
            }

            if (c.category > 0)
                sb.Append(" and datas.genre & " + toInt(c.category) + " = " + toInt(c.category));

            if (c.omega != null && c.omega[0] > 0)
            {
                if (c.omega[1] > 0)
                    sb.Append(" and datas.category & " + toInt((long)c.omega[1]) + " = " + toInt((long)c.omega[1]));
                if (c.omega[2] > 0)
                    sb.Append(" and datas.support & " + toInt((long)c.omega[2]) + " = " + toInt((long)c.omega[2]));
                if (c.omega[3] > 0 && c.omega[3] < 253402207200)
                    sb.Append(" and datas.tcgdate = " + toInt(DateTime.Parse(c.GetDate(1)).Ticks / 10000000));
                if (c.omega[4] > 0 && c.omega[4] < 253402207200)
                    sb.Append(" and datas.ocgdate = " + toInt(DateTime.Parse(c.GetDate()).Ticks / 10000000));
            }

            if (c.atk == -1)
            {
                sb.Append(" and datas.type & 1 = 1 and datas.atk = 0");
            }
            else if (c.atk < 0 || c.atk > 0)
            {
                sb.Append(" and datas.atk = " + c.atk.ToString());
            }

            if (c.IsType(Info.CardType.TYPE_LINK))
            {
                sb.Append(" and datas.def &" + c.def.ToString() + "=" + c.def.ToString());
            }
            else
            {
                if (c.def == -1)
                {
                    sb.Append(" and datas.type & 1 = 1 and datas.def = 0");
                }
                else if (c.def < 0 || c.def > 0)
                {
                    sb.Append(" and datas.def = " + c.def.ToString());
                }
            }

            if (c.id > 0 && c.alias > 0)
            {
                sb.Append(" and datas.id BETWEEN " + c.alias.ToString() + " and " + c.id.ToString());
            }
            else if (c.id > 0)
            {
                sb.Append(" and ( datas.id=" + c.id.ToString() + " or datas.alias=" + c.id.ToString() + ") ");
            }
            else if (c.alias > 0)
            {
                sb.Append(" and datas.alias= " + c.alias.ToString());
            }

            return sb.ToString();

        }
        public static string GetSelectSQL(Card c)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT datas.*,texts.* FROM datas,texts WHERE datas.id=texts.id ");
            if (c == null)
            {
                return sb.ToString();
            }

            if (!string.IsNullOrEmpty(c.name))
            {
                if (c.name.IndexOf("%%") >= 0)
                {
                    c.name = c.name.Replace("%%", "%");
                }
                else
                {
                    c.name = "%" + c.name.Replace("%", "/%").Replace("_", "/_") + "%";
                }

                sb.Append(" and texts.name like '" + c.name.Replace("'", "''") + "' ");
            }
            if (!string.IsNullOrEmpty(c.desc))
            {
                sb.Append(" and texts.desc like '%" + c.desc.Replace("'", "''") + "%' ");
            }

            if (c.ot > 0)
            {
                sb.Append(" and datas.ot = " + c.ot.ToString());
            }

            if (c.attribute > 0)
            {
                sb.Append(" and datas.attribute = " + c.attribute.ToString());
            }

            if ((c.level & 0xff) > 0)
            {
                sb.Append(" and (datas.level & 255) = " + toInt(c.level & 0xff));
            }

            if ((c.level & 0xff000000) > 0)
            {
                sb.Append(" and (datas.level & 4278190080) = " + toInt(c.level & 0xff000000));
            }

            if ((c.level & 0xff0000) > 0)
            {
                sb.Append(" and (datas.level & 16711680) = " + toInt(c.level & 0xff0000));
            }

            if (c.race > 0)
            {
                sb.Append(" and datas.race = " + toInt(c.race));
            }

            if (c.type > 0)
            {
                sb.Append(" and datas.type & " + toInt(c.type) + " = " + toInt(c.type));
            }

            if (c.category > 0)
            {
                sb.Append(" and datas.category & " + toInt(c.category) + " = " + toInt(c.category));
            }

            if (c.atk == -1)
            {
                sb.Append(" and datas.type & 1 = 1 and datas.atk = 0");
            }
            else if (c.atk < 0 || c.atk > 0)
            {
                sb.Append(" and datas.atk = " + c.atk.ToString());
            }

            if (c.IsType(Info.CardType.TYPE_LINK))
            {
                sb.Append(" and datas.def &" + c.def.ToString() + "=" + c.def.ToString());
            }
            else
            {
                if (c.def == -1)
                {
                    sb.Append(" and datas.type & 1 = 1 and datas.def = 0");
                }
                else if (c.def < 0 || c.def > 0)
                {
                    sb.Append(" and datas.def = " + c.def.ToString());
                }
            }

            if (c.id > 0 && c.alias > 0)
            {
                sb.Append(" and datas.id BETWEEN " + c.alias.ToString() + " and " + c.id.ToString());
            }
            else if (c.id > 0)
            {
                sb.Append(" and ( datas.id=" + c.id.ToString() + " or datas.alias=" + c.id.ToString() + ") ");
            }
            else if (c.alias > 0)
            {
                sb.Append(" and datas.alias= " + c.alias.ToString());
            }

            return sb.ToString();

        }
        #endregion

        #region 插入
        /// <summary>
        /// 转换为插入语句
        /// </summary>
        /// <param name="c">卡片数据</param>
        /// <param name="ignore"></param>
        /// <returns>SQL语句</returns>
        public static string OmegaGetInsertSQL(Card c, bool ignore, bool hex = false)
        {
            StringBuilder st = new StringBuilder();
            if (ignore)
            {
                st.Append("INSERT or ignore into datas values(");
            }
            else
            {
                st.Append("INSERT or replace into datas values(");
            }

            st.Append(c.id.ToString()); st.Append(",");
            st.Append(c.ot.ToString()); st.Append(",");
            st.Append(c.alias.ToString()); st.Append(",");
            if (hex)
            {
                st.Append("0x" + c.setcode.ToString("x")); st.Append(",");
                st.Append("0x" + c.type.ToString("x")); st.Append(",");
            }
            else
            {
                st.Append(c.setcode.ToString()); st.Append(",");
                st.Append(c.type.ToString()); st.Append(",");
            }
            st.Append(c.atk.ToString()); ; st.Append(",");
            st.Append(c.def.ToString()); st.Append(",");
            if (hex)
            {
                st.Append("0x" + c.level.ToString("x")); st.Append(",");
                st.Append("0x" + c.race.ToString("x")); st.Append(",");
                st.Append("0x" + c.attribute.ToString("x")); st.Append(",");
                if (c.omega[0] > 0) st.Append("0x" + c.omega[1].ToString("x")); else st.Append("0x0");
                st.Append(",");
                st.Append("0x" + c.category.ToString("x"));
                if (c.omega[0] > 0)
                {
                    st.Append(",null,"); st.Append("0x" + c.omega[2].ToString("x"));
                    st.Append(","); st.Append(c.omega[3].ToString());
                    st.Append(","); st.Append(c.omega[4].ToString());
                }
                else st.Append(",0x0,null,253402207200,253402207200");
            }
            else
            {
                st.Append(c.level.ToString()); st.Append(",");
                st.Append(c.race.ToString()); st.Append(",");
                st.Append(c.attribute.ToString()); st.Append(",");
                if (c.omega[0] > 0) st.Append(c.omega[1].ToString()); else st.Append("0");
                st.Append(",");
                st.Append(c.category.ToString());
                if (c.omega[0] > 0)
                {
                    st.Append(",null,"); st.Append(c.omega[2].ToString());
                    st.Append(","); st.Append(c.omega[3].ToString());
                    st.Append(","); st.Append(c.omega[4].ToString());
                }
                else st.Append(",0,null,253402207200,253402207200");
            }
            st.Append(")");
            if (ignore)
            {
                st.Append(";\nINSERT or ignore into texts values(");
            }
            else
            {
                st.Append(";\nINSERT or replace into texts values(");
            }

            st.Append(c.id.ToString()); st.Append(",'");
            st.Append(c.name.Replace("'", "''")); st.Append("','");
            st.Append(c.desc.Replace("'", "''"));
            for (int i = 0; i < 0x10; i++)
            {
                st.Append("','"); st.Append(c.Str[i].Replace("'", "''"));
            }
            st.Append("');");
            string sql = st.ToString();
            return sql;
        }
        /// <summary>
        /// 转换为插入语句
        /// </summary>
        /// <param name="c">卡片数据</param>
        /// <param name="ignore"></param>
        /// <returns>SQL语句</returns>
        public static string GetInsertSQL(Card c, bool ignore, bool hex = false)
        {
            StringBuilder st = new StringBuilder();
            if (ignore)
            {
                st.Append("INSERT or ignore into datas values(");
            }
            else
            {
                st.Append("INSERT or replace into datas values(");
            }

            st.Append(c.id.ToString()); st.Append(",");
            st.Append(c.ot.ToString()); st.Append(",");
            st.Append(c.alias.ToString()); st.Append(",");
            if (hex)
            {
                st.Append("0x" + c.setcode.ToString("x")); st.Append(",");
                st.Append("0x" + c.type.ToString("x")); st.Append(",");
            }
            else
            {
                st.Append(c.setcode.ToString()); st.Append(",");
                st.Append(c.type.ToString()); st.Append(",");
            }
            st.Append(c.atk.ToString()); ; st.Append(",");
            st.Append(c.def.ToString()); st.Append(",");
            if (hex)
            {
                st.Append("0x" + c.level.ToString("x")); st.Append(",");
                st.Append("0x" + c.race.ToString("x")); st.Append(",");
                st.Append("0x" + c.attribute.ToString("x")); st.Append(",");
                st.Append("0x" + c.category.ToString("x"));
            }
            else
            {
                st.Append(c.level.ToString()); st.Append(",");
                st.Append(c.race.ToString()); st.Append(",");
                st.Append(c.attribute.ToString()); st.Append(",");
                st.Append(c.category.ToString());
            }
            st.Append(")");
            if (ignore)
            {
                st.Append(";\nINSERT or ignore into texts values(");
            }
            else
            {
                st.Append(";\nINSERT or replace into texts values(");
            }

            st.Append(c.id.ToString()); st.Append(",'");
            st.Append(c.name.Replace("'", "''")); st.Append("','");
            st.Append(c.desc.Replace("'", "''"));
            for (int i = 0; i < 0x10; i++)
            {
                st.Append("','"); st.Append(c.Str[i].Replace("'", "''"));
            }
            st.Append("');");
            string sql = st.ToString();
            return sql;
        }
        #endregion

        #region 更新
        /// <summary>
        /// 转换为更新语句
        /// </summary>
        /// <param name="c">卡片数据</param>
        /// <returns>SQL语句</returns>
        public static string OmegaGetUpdateSQL(Card c)
        {
            StringBuilder st = new StringBuilder();
            st.Append("update datas set ot="); st.Append(c.ot.ToString());
            st.Append(",alias="); st.Append(c.alias.ToString());
            st.Append(",setcode="); st.Append(c.setcode.ToString());
            st.Append(",type="); st.Append(c.type.ToString());
            st.Append(",atk="); st.Append(c.atk.ToString());
            st.Append(",def="); st.Append(c.def.ToString());
            st.Append(",level="); st.Append(c.level.ToString());
            st.Append(",race="); st.Append(c.race.ToString());
            st.Append(",attribute="); st.Append(c.attribute.ToString());
            st.Append(",category=");
            if (c.omega[0] > 0)
            {
                st.Append(c.omega[1].ToString());
                st.Append(",support="); st.Append(c.omega[2].ToString());
                st.Append(",ocgdate="); st.Append(((DateTime.Parse(c.GetDate()).Ticks - new DateTime(1970, 1, 1).Ticks) / 10000000).ToString());
                st.Append(",tcgdate="); st.Append(((DateTime.Parse(c.GetDate(1)).Ticks - new DateTime(1970, 1, 1).Ticks) / 10000000).ToString());
                st.Append(",genre=");
            }
            st.Append(c.category.ToString());
            st.Append(" where id="); st.Append(c.id.ToString());
            st.Append("; update texts set name='"); st.Append(c.name.Replace("'", "''"));
            st.Append("',desc='"); st.Append(c.desc.Replace("'", "''")); st.Append("', ");
            for (int i = 0; i < 0x10; i++)
            {
                st.Append("str"); st.Append((i + 1).ToString()); st.Append("='");
                st.Append(c.Str[i].Replace("'", "''"));
                if (i < 15)
                {
                    st.Append("',");
                }
            }
            st.Append("' where id="); st.Append(c.id.ToString());
            st.Append(";");
            string sql = st.ToString();
            return sql;
        }
        /// <summary>
        /// 转换为更新语句
        /// </summary>
        /// <param name="c">卡片数据</param>
        /// <returns>SQL语句</returns>
        public static string GetUpdateSQL(Card c)
        {
            StringBuilder st = new StringBuilder();
            st.Append("update datas set ot="); st.Append(c.ot.ToString());
            st.Append(",alias="); st.Append(c.alias.ToString());
            st.Append(",setcode="); st.Append(c.setcode.ToString());
            st.Append(",type="); st.Append(c.type.ToString());
            st.Append(",atk="); st.Append(c.atk.ToString());
            st.Append(",def="); st.Append(c.def.ToString());
            st.Append(",level="); st.Append(c.level.ToString());
            st.Append(",race="); st.Append(c.race.ToString());
            st.Append(",attribute="); st.Append(c.attribute.ToString());
            st.Append(",category="); st.Append(c.category.ToString());
            st.Append(" where id="); st.Append(c.id.ToString());
            st.Append("; update texts set name='"); st.Append(c.name.Replace("'", "''"));
            st.Append("',desc='"); st.Append(c.desc.Replace("'", "''")); st.Append("', ");
            for (int i = 0; i < 0x10; i++)
            {
                st.Append("str"); st.Append((i + 1).ToString()); st.Append("='");
                st.Append(c.Str[i].Replace("'", "''"));
                if (i < 15)
                {
                    st.Append("',");
                }
            }
            st.Append("' where id="); st.Append(c.id.ToString());
            st.Append(";");
            string sql = st.ToString();
            return sql;
        }
        #endregion

        #region 删除
        /// <summary>
        /// 转换删除语句
        /// </summary>
        /// <param name="c">卡片密码</param>
        /// <returns>SQL语句</returns>
        public static string GetDeleteSQL(Card c)
        {
            string id = c.id.ToString();
            return "Delete from datas where id=" + id + ";Delete from texts where id=" + id + ";";
        }
        #endregion
        #endregion


        public static void ExportSql(string file, params Card[] cards)
        {
            using (FileStream fs = new FileStream(file, FileMode.Create, FileAccess.Write))
            {
                StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);
                foreach (Card c in cards)
                {
                    sw.WriteLine(GetInsertSQL(c, false, true));
                }
                sw.Close();
            }
        }

        public static CardPack FindPack(string db, long id)
        {
            CardPack cardpack = null;
            if (File.Exists(db) && id >= 0)
            {
                using (SQLiteConnection sqliteconn = new SQLiteConnection(@"Data Source=" + db))
                {
                    sqliteconn.Open();
                    using (SQLiteCommand sqlitecommand = new SQLiteCommand(sqliteconn))
                    {
                        sqlitecommand.CommandText = "select id,pack_id,pack,rarity,date from pack where id=" + id + " order by date desc";
                        using (SQLiteDataReader reader = sqlitecommand.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                cardpack = new CardPack(id)
                                {
                                    pack_id = reader.GetString(1),
                                    pack_name = reader.GetString(2),
                                    rarity = reader.GetString(3),
                                    date = reader.GetString(4)
                                };
                            }
                            reader.Close();
                        }
                    }
                    sqliteconn.Close();
                }
            }
            return cardpack;
        }
    }
}
