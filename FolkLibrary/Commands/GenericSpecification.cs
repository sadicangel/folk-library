﻿using Ardalis.Specification;
using FolkLibrary.Models;

namespace FolkLibrary.Commands;

public sealed class GenericSpecification<T> : Specification<T>
    where T : Item
{
    public GenericSpecification(Action<ISpecificationBuilder<T>> configure) => configure(Query);
}
