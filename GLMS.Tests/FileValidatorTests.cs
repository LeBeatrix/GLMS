using GLMS.Web.Services;

namespace GLMS.Tests
{
    public class FileValidatorTests
    {
        [Fact]
        public void IsPdfFile_WithPdf_ReturnsTrue()
        {
            var validator = new FileValidator();

            var result = validator.IsPdfFile("contract.pdf");

            Assert.True(result);
        }

        [Fact]
        public void IsPdfFile_WithExe_ReturnsFalse()
        {
            var validator = new FileValidator();

            var result = validator.IsPdfFile("virus.exe");

            Assert.False(result);
        }

        [Fact]
        public void IsPdfFile_EmptyName_ReturnsFalse()
        {
            var validator = new FileValidator();

            var result = validator.IsPdfFile("");

            Assert.False(result);
        }
    }
}