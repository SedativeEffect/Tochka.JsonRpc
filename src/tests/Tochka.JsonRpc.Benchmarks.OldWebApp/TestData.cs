﻿namespace Tochka.JsonRpc.Benchmarks.OldWebApp;

public record TestData(bool BoolField, string StringField, int IntField, double DoubleField, TestEnum EnumField, string? NullableField, string? NotRequiredField = null, TestData? NestedField = null);

public enum TestEnum
{
    One,
    Two
}
