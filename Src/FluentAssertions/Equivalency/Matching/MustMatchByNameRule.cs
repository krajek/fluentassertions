using System.Reflection;
using FluentAssertions.Common;
using FluentAssertions.Execution;

namespace FluentAssertions.Equivalency.Matching
{
    /// <summary>
    /// Requires the expectation object to have a member with the exact same name.
    /// </summary>
    internal class MustMatchByNameRule : IMemberMatchingRule
    {
        public SelectedMemberInfo Match(SelectedMemberInfo expectedMember, object subject, string memberPath, IEquivalencyAssertionOptions config)
        {
            SelectedMemberInfo compareeSelectedMemberInfo = null;

            if (config.IncludeProperties)
            {
                compareeSelectedMemberInfo = SelectedMemberInfo.Create(subject.GetType()
                    .FindProperty(expectedMember.Name, expectedMember.MemberType));
            }

            if ((compareeSelectedMemberInfo is null) && config.IncludeFields)
            {
                compareeSelectedMemberInfo = SelectedMemberInfo.Create(subject.GetType()
                    .FindField(expectedMember.Name, expectedMember.MemberType));
            }

            if ((compareeSelectedMemberInfo is null || !config.UseRuntimeTyping) && ExpectationImplementsMemberExplicitly(subject, expectedMember))
            {
                compareeSelectedMemberInfo = expectedMember;
            }

            if (compareeSelectedMemberInfo is null)
            {
                string path = (memberPath.Length > 0) ? memberPath + "." : "member ";

                Execute.Assertion.FailWith(
                    "Expectation has " + path + expectedMember.Name + " that the other object does not have.");
            }

            return compareeSelectedMemberInfo;
        }

        private static bool ExpectationImplementsMemberExplicitly(object expectation, SelectedMemberInfo subjectMember)
        {
            return subjectMember.DeclaringType.IsInstanceOfType(expectation);
        }

        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return "Match member by name (or throw)";
        }
    }
}
