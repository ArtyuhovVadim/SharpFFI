using System.Runtime.CompilerServices;

namespace SharpFFI.Primitives.Tests;

public class Bool8Tests
{
    [Fact]
    public void Ctor_FromBool_SetsZeroOrOne()
    {
        // Arrange
        var falseInput = false;
        var trueInput = true;

        // Act
        var b8False = new Bool8(falseInput);
        var b8True = new Bool8(trueInput);

        // Assert
        b8False.Value.ShouldBe((byte)0);
        b8True.Value.ShouldBe((byte)1);
    }

    [Fact]
    public void Ctor_FromInteger_NormalizesNonZeroToOne()
    {
        // Arrange
        byte[] testBytes = [0, 1, 2, 127, 255];

        // Act & Assert
        foreach (var b in testBytes)
        {
            var bool8 = new Bool8(b);
            var expected = b == 0 ? (byte)0 : (byte)1;
            bool8.Value.ShouldBe(expected);
        }
    }

    [Fact]
    public void Ctor_CrossConversion_PreservesTruthiness()
    {
        // Arrange
        var b32False = new Bool32(false);
        var b32True = new Bool32(true);

        // Act
        var b8False = new Bool8(b32False);
        var b8True = new Bool8(b32True);

        // Assert
        b8False.Value.ShouldBe((byte)0);
        b8True.Value.ShouldBe((byte)1);
    }

    [Fact]
    public void UnnormalizedMemory_BehavesAsCanonicalTrue()
    {
        // Arrange
        byte raw8 = 100;
        ref var b8Unnormalized = ref Unsafe.As<byte, Bool8>(ref raw8);
        var b8Canonical = new Bool8(true);

        byte raw8_2 = 2;
        ref var b8Unnormalized2 = ref Unsafe.As<byte, Bool8>(ref raw8_2);

        // Act & Assert
        b8Unnormalized.Value.ShouldBe((byte)100);
        b8Unnormalized.ShouldBe(b8Canonical);
        b8Unnormalized2.ShouldBe(b8Canonical);
        b8Unnormalized.GetHashCode().ShouldBe(1);
        b8Unnormalized.CompareTo(b8Canonical).ShouldBe(0);
        b8Unnormalized.ToString().ShouldBe("True");
    }

    [Fact]
    public unsafe void MemoryLayout_SizeOf_MatchesAbi()
    {
        // Arrange & Act
        var size = sizeof(Bool8);

        // Assert
        size.ShouldBe(1);
    }

    [Fact]
    public void Equality_AllCombinations_ReturnsExpected()
    {
        // Arrange
        var b8F1 = new Bool8(false);
        var b8F2 = new Bool8(false);
        var b8T1 = new Bool8(true);
        var b8T2 = new Bool8(true);

        // Act & Assert
        b8F1.ShouldBe(b8F2);
        b8F1.Equals((object)b8F2).ShouldBeTrue();

        b8F1.ShouldNotBe(b8T1);
        b8F1.Equals((object)b8T1).ShouldBeFalse();

        b8T1.ShouldNotBe(b8F1);
        b8T1.Equals((object)b8F1).ShouldBeFalse();

        b8T1.ShouldBe(b8T2);
        b8T1.Equals((object)b8T2).ShouldBeTrue();

        b8T1.Equals(null).ShouldBeFalse();
        b8T1.Equals("wrong_type").ShouldBeFalse();
    }

    [Fact]
    public void GetHashCode_ReturnsZeroOrOne()
    {
        // Arrange
        var b8False = new Bool8(false);
        var b8True = new Bool8(true);

        // Act
        var hashFalse = b8False.GetHashCode();
        var hashTrue = b8True.GetHashCode();

        // Assert
        hashFalse.ShouldBe(0);
        hashTrue.ShouldBe(1);
    }

    [Fact]
    public void Comparison_OperatorsAndCompareTo_FollowBooleanOrder()
    {
        // Arrange
        var b8F1 = new Bool8(false);
        var b8F2 = new Bool8(false);
        var b8T1 = new Bool8(true);
        var b8T2 = new Bool8(true);

        // Act & Assert
        b8F1.ShouldBeLessThan(b8T1);
        b8F1.ShouldBeLessThanOrEqualTo(b8T1);
        b8T1.ShouldBeGreaterThan(b8F1);
        b8T1.ShouldBeGreaterThanOrEqualTo(b8F1);

        b8F1.ShouldBeLessThanOrEqualTo(b8F2);
        b8F1.ShouldBeGreaterThanOrEqualTo(b8F2);
        b8F1.CompareTo(b8F2).ShouldBe(0);

        b8T1.ShouldBeLessThanOrEqualTo(b8T2);
        b8T1.ShouldBeGreaterThanOrEqualTo(b8T2);
        b8T1.CompareTo(b8T2).ShouldBe(0);

        b8F1.CompareTo(b8T1).ShouldBeNegative();
        b8T1.CompareTo(b8F1).ShouldBePositive();
    }

    [Fact]
    public void CompareTo_Object_HandlesNullAndInvalidType()
    {
        // Arrange
        var b8 = new Bool8(true);

        // Act
        var nullCompare = b8.CompareTo((object?)null);

        // Assert
        nullCompare.ShouldBe(1);
        Should.Throw<ArgumentException>(() => b8.CompareTo("wrong_type"));
    }

    [Fact]
    public void ImplicitConversions_BoolAndCrossType_Roundtrip()
    {
        // Arrange
        var managedTrue = true;
        var managedFalse = false;

        // Act
        Bool8 b8FromTrue = managedTrue;
        bool boolFromB8True = b8FromTrue;

        Bool8 b8FromFalse = managedFalse;
        bool boolFromB8False = b8FromFalse;

        Bool32 b32FromB8 = b8FromTrue;
        Bool8 b8FromB32 = b32FromB8;

        // Assert
        b8FromTrue.Value.ShouldBe((byte)1);
        boolFromB8True.ShouldBeTrue();

        b8FromFalse.Value.ShouldBe((byte)0);
        boolFromB8False.ShouldBeFalse();

        b32FromB8.Value.ShouldBe(1u);
        b8FromB32.Value.ShouldBe((byte)1);
    }

    [Fact]
    public void LogicalOperators_NotTrueFalse_WorkInShortCircuit()
    {
        // Arrange
        var b8True = new Bool8(true);
        var b8False = new Bool8(false);

        // Act & Assert - Unary !
        (!b8True).ShouldBeFalse();
        (!b8False).ShouldBeTrue();

        // Act & Assert - If branches
        var ifTrueExecuted = false;
        if (b8True)
            ifTrueExecuted = true;
        ifTrueExecuted.ShouldBeTrue();

        var ifFalseExecuted = false;
        if (b8False)
            ifFalseExecuted = true;
        ifFalseExecuted.ShouldBeFalse();

        // Act & Assert - Short-circuiting
        var sideEffectCalled = false;
        bool SideEffect()
        {
            sideEffectCalled = true;
            return true;
        }

        if (b8False && SideEffect())
        {
        }
        sideEffectCalled.ShouldBeFalse();

        if (b8True || SideEffect())
        {
        }
        sideEffectCalled.ShouldBeFalse();
    }

    [Fact]
    public void EqualityOperators_DirectInvocation_ReturnsExpected()
    {
        // Arrange
        var b8False1 = new Bool8(false);
        var b8False2 = new Bool8(false);
        var b8True1 = new Bool8(true);
        var b8True2 = new Bool8(true);

        // Act
        var eqSameFalse = b8False1 == b8False2;
        var eqSameTrue = b8True1 == b8True2;
        var eqDiff = b8False1 == b8True1;
        var neqDiff = b8False1 != b8True1;
        var neqSame = b8False1 != b8False2;

        // Assert
        eqSameFalse.ShouldBeTrue();
        eqSameTrue.ShouldBeTrue();
        eqDiff.ShouldBeFalse();
        neqDiff.ShouldBeTrue();
        neqSame.ShouldBeFalse();
    }

    [Fact]
    public void RelationalOperators_DirectInvocation_ReturnsExpected()
    {
        // Arrange
        var b8False = new Bool8(false);
        var b8True = new Bool8(true);

        // Act
        var lt = b8False < b8True;
        var lteSame = b8False <= b8False;
        var lteDiff = b8False <= b8True;
        var gt = b8True > b8False;
        var gteSame = b8True >= b8True;
        var gteDiff = b8True >= b8False;

        var notLt = b8True < b8False;
        var notGt = b8False > b8True;

        // Assert
        lt.ShouldBeTrue();
        lteSame.ShouldBeTrue();
        lteDiff.ShouldBeTrue();
        gt.ShouldBeTrue();
        gteSame.ShouldBeTrue();
        gteDiff.ShouldBeTrue();
        notLt.ShouldBeFalse();
        notGt.ShouldBeFalse();
    }

    [Fact]
    public void UserDefinedTrueFalseOperators_ViaReflection_ReturnExpected()
    {
        // Arrange
        var b8False = new Bool8(false);
        var b8True = new Bool8(true);
        var opTrue = typeof(Bool8).GetMethod("op_True", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)!;
        var opFalse = typeof(Bool8).GetMethod("op_False", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)!;

        // Act
        var trueOnTrue = (bool)opTrue.Invoke(null, [b8True])!;
        var trueOnFalse = (bool)opTrue.Invoke(null, [b8False])!;
        var falseOnFalse = (bool)opFalse.Invoke(null, [b8False])!;
        var falseOnTrue = (bool)opFalse.Invoke(null, [b8True])!;

        // Assert
        trueOnTrue.ShouldBeTrue();
        trueOnFalse.ShouldBeFalse();
        falseOnFalse.ShouldBeTrue();
        falseOnTrue.ShouldBeFalse();
    }
}
