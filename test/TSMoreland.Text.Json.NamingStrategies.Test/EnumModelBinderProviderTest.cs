//
// Copyright © 2022 Terry Moreland
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), 
// to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
// and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

namespace TSMoreland.Text.Json.NamingStrategies.Test;

public sealed class EnumModelBinderProviderTest
{
    private readonly Mock<ModelBinderProviderContext> _context = new();
    private readonly Mock<ModelMetadata> _sampleValueModelMetaData = new(ModelMetadataIdentity.ForType(typeof(SampleValue)));
    private readonly Mock<ModelMetadata> _sampleFlagsModelMetaData = new(ModelMetadataIdentity.ForType(typeof(SampleFlags)));
    private readonly Mock<ModelMetadata> _nonEnumModelMetaData = new(ModelMetadataIdentity.ForType(typeof(List<>)));

    [Fact]
    public void GetBinder_ReturnsNonNull_WhenContextModelTypeIsEnumWithoutFlags()
    {
        _context.SetupGet(m => m.Metadata).Returns(_sampleValueModelMetaData.Object);
        EnumModelBinderProvider provider = new();

        IModelBinder? actual = provider.GetBinder(_context.Object);

        actual.Should().NotBeNull();
    }

    [Fact]
    public void GetBinder_ReturnsNull_WhenContextModelTypeIsEnumWithFlags()
    {
        _context.SetupGet(m => m.Metadata).Returns(_sampleFlagsModelMetaData.Object);
        EnumModelBinderProvider provider = new();

        IModelBinder? actual = provider.GetBinder(_context.Object);

        actual.Should().BeNull();
    }

    [Fact]
    public void GetBinder_ReturnsNull_WhenContextModelTypeIsNotEnum()
    {
        _context.SetupGet(m => m.Metadata).Returns(_nonEnumModelMetaData.Object);
        EnumModelBinderProvider provider = new();

        IModelBinder? actual = provider.GetBinder(_context.Object);

        actual.Should().BeNull();
    }
}
