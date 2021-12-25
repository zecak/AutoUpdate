using CommandLine;
using CommandLine.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anye.Soft.AutoUpdate.Exec
{
    public class SentenceBuilderCN : SentenceBuilder
    {
        public override Func<string> RequiredWord
        {
            get { return () => "必填"; }
        }

        public override Func<string> ErrorsHeadingText
        {
            get { return () => "错误信息:"; }
        }

        public override Func<string> UsageHeadingText
        {
            get { return () => "用法:"; }
        }

        public override Func<string> OptionGroupWord
        {
            get { return () => "组"; }
        }

        public override Func<bool, string> HelpCommandText
        {
            get
            {
                return isOption => isOption
                    ? "显示命令帮助信息"
                    : "显示关于特定命令的更多信息";
            }
        }

        public override Func<bool, string> VersionCommandText
        {
            get { return _ => "显示版本信息"; }
        }

        public override Func<Error, string> FormatError
        {
            get
            {
                return error =>
                {
                    switch (error.Tag)
                    {
                        case ErrorType.BadFormatTokenError:
                            return "令牌 '".JoinTo(((BadFormatTokenError)error).Token, "' 没有经过验证");
                        case ErrorType.MissingValueOptionError:
                            return "选项 '".JoinTo(((MissingValueOptionError)error).NameInfo.NameText,
                                "' 无值");
                        case ErrorType.UnknownOptionError:
                            return "选项 '".JoinTo(((UnknownOptionError)error).Token, "' 未知");
                        case ErrorType.MissingRequiredOptionError:
                            var errMisssing = ((MissingRequiredOptionError)error);
                            return errMisssing.NameInfo.Equals(NameInfo.EmptyName)
                                       ? "缺少一个未绑定到选项名称的必需值"
                                       : "必填选项 '".JoinTo(errMisssing.NameInfo.NameText, "' 缺少");
                        case ErrorType.BadFormatConversionError:
                            var badFormat = ((BadFormatConversionError)error);
                            return badFormat.NameInfo.Equals(NameInfo.EmptyName)
                                       ? "未绑定到选项名称的值定义为错误的格式"
                                       : "选项 '".JoinTo(badFormat.NameInfo.NameText, "' 定义的格式错误");
                        case ErrorType.SequenceOutOfRangeError:
                            var seqOutRange = ((SequenceOutOfRangeError)error);
                            return seqOutRange.NameInfo.Equals(NameInfo.EmptyName)
                                       ? "不绑定到选项名称的序列值定义的项比需要的少"
                                       : "序列选项 '".JoinTo(seqOutRange.NameInfo.NameText,
                                            "' 定义的条目比需要的少还是多");
                        case ErrorType.BadVerbSelectedError:
                            return "动词 '".JoinTo(((BadVerbSelectedError)error).Token, "' 没有经过验证");
                        case ErrorType.NoVerbSelectedError:
                            return "无动词被选择";
                        case ErrorType.RepeatedOptionError:
                            return "选项 '".JoinTo(((RepeatedOptionError)error).NameInfo.NameText,
                                "' 被定义多次");
                        case ErrorType.SetValueExceptionError:
                            var setValueError = (SetValueExceptionError)error;
                            return "选项设置值出错 '".JoinTo(setValueError.NameInfo.NameText, "': ", setValueError.Exception.Message);
                        case ErrorType.MissingGroupOptionError:
                            var missingGroupOptionError = (MissingGroupOptionError)error;
                            return "至少一组选项 '".JoinTo(
                                missingGroupOptionError.Group,
                                "' (",
                                string.Join(", ", missingGroupOptionError.Names.Select(n => n.NameText)),
                                ") 是必须的");
                        case ErrorType.GroupOptionAmbiguityError:
                            var groupOptionAmbiguityError = (GroupOptionAmbiguityError)error;
                            return "SetName和Group都不允许在选项中: (".JoinTo(groupOptionAmbiguityError.Option.NameText, ")");
                        case ErrorType.MultipleDefaultVerbsError:
                            return MultipleDefaultVerbsError.ErrorMessage;

                    }
                    throw new InvalidOperationException();
                };
            }
        }

        public override Func<IEnumerable<MutuallyExclusiveSetError>, string> FormatMutuallyExclusiveSetErrors
        {
            get
            {
                return errors =>
                {
                    var bySet = from e in errors
                                group e by e.SetName into g
                                select new { SetName = g.Key, Errors = g.ToList() };

                    var msgs = bySet.Select(
                        set =>
                        {
                            var names = string.Join(
                                string.Empty,
                                (from e in set.Errors select "'".JoinTo(e.NameInfo.NameText, "', ")).ToArray());
                            var namesCount = set.Errors.Count();

                            var incompat = string.Join(
                                string.Empty,
                                (from x in
                                     (from s in bySet where !s.SetName.Equals(set.SetName) from e in s.Errors select e)
                                    .Distinct()
                                 select "'".JoinTo(x.NameInfo.NameText, "', ")).ToArray());

                            return
                                new StringBuilder("Option")
                                        .AppendWhen(namesCount > 1, "s")
                                        .Append(": ")
                                        .Append(names.Substring(0, names.Length - 2))
                                        .Append(' ')
                                        .AppendIf(namesCount > 1, "are", "is")
                                        .Append(" not compatible with: ")
                                        .Append(incompat.Substring(0, incompat.Length - 2))
                                        .Append('.')
                                    .ToString();
                        }).ToArray();
                    return string.Join(Environment.NewLine, msgs);
                };
            }
        }
    }
}
