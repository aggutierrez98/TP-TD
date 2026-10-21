using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ABSTRACCIONES;
using BE;
using SERVICIOS;

namespace SERVICIOS.Tests
{
    [TestClass]
    public sealed class IntegridadServiceTests
    {
        private readonly IntegridadService _service = new IntegridadService();

        [TestMethod]
        public void CalcularDVH_SameEntity_ReturnsSameHash()
        {
            TestEntity entity = new TestEntity { Id = 1, Name = "admin", Active = true };

            string first = _service.CalcularDVH(entity);
            string second = _service.CalcularDVH(entity);

            Assert.AreEqual(first, second);
        }

        [TestMethod]
        public void CalcularDVH_ChangedVerifiableProperty_ReturnsDifferentHash()
        {
            TestEntity first = new TestEntity { Id = 1, Name = "admin", Active = true };
            TestEntity second = new TestEntity { Id = 1, Name = "other", Active = true };

            Assert.AreNotEqual(_service.CalcularDVH(first), _service.CalcularDVH(second));
        }

        [TestMethod]
        public void CalcularDVH_NullEntity_ThrowsArgumentNullException()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => _service.CalcularDVH<TestEntity>(null));
        }

        [TestMethod]
        public void CalcularDVV_SameList_ReturnsSameHash()
        {
            List<string> dvhs = CrearDVHs();

            string first = _service.CalcularDVV(dvhs);
            string second = _service.CalcularDVV(dvhs);

            Assert.AreEqual(first, second);
        }

        [TestMethod]
        public void CalcularDVVIncremental_Insert_EqualsFullRecalculation()
        {
            List<string> existing = CrearDVHs();
            string newDvh = _service.CalcularDVH(new TestEntity { Id = 3, Name = "guest", Active = true });
            string currentDvv = _service.CalcularDVV(existing);

            string incremental = _service.CalcularDVVIncremental(currentDvv, null, newDvh);
            existing.Add(newDvh);

            Assert.AreEqual(_service.CalcularDVV(existing), incremental);
        }

        [TestMethod]
        public void CalcularDVVIncremental_Update_EqualsFullRecalculation()
        {
            List<string> existing = CrearDVHs();
            string oldDvh = existing[0];
            string newDvh = _service.CalcularDVH(new TestEntity { Id = 1, Name = "renamed", Active = true });
            string currentDvv = _service.CalcularDVV(existing);

            string incremental = _service.CalcularDVVIncremental(currentDvv, oldDvh, newDvh);
            existing[0] = newDvh;

            Assert.AreEqual(_service.CalcularDVV(existing), incremental);
        }

        [TestMethod]
        public void CalcularDVVIncremental_Delete_EqualsFullRecalculation()
        {
            List<string> existing = CrearDVHs();
            string removed = existing[0];
            string currentDvv = _service.CalcularDVV(existing);

            string incremental = _service.CalcularDVVIncremental(currentDvv, removed, null);
            existing.RemoveAt(0);

            Assert.AreEqual(_service.CalcularDVV(existing), incremental);
        }

        [TestMethod]
        public void CalcularDVVIncremental_NullCurrentDvv_StartsFromEmptyAccumulator()
        {
            string dvh = _service.CalcularDVH(new TestEntity { Id = 1, Name = "admin", Active = true });

            string incremental = _service.CalcularDVVIncremental(null, null, dvh);

            Assert.AreEqual(_service.CalcularDVV(new[] { dvh }), incremental);
        }

        [TestMethod]
        public void CalcularDVVIncremental_InvalidBase64_ThrowsArgumentException()
        {
            Assert.ThrowsExactly<ArgumentException>(() => _service.CalcularDVVIncremental("not-base64", null, null));
        }

        [TestMethod]
        public void CalcularDVVIncremental_InvalidByteLength_ThrowsArgumentException()
        {
            string invalidLength = Convert.ToBase64String(new byte[] { 1, 2, 3 });

            Assert.ThrowsExactly<ArgumentException>(() => _service.CalcularDVVIncremental(invalidLength, null, null));
        }

        private List<string> CrearDVHs()
        {
            return new List<string>
            {
                _service.CalcularDVH(new TestEntity { Id = 1, Name = "admin", Active = true }),
                _service.CalcularDVH(new TestEntity { Id = 2, Name = "basic", Active = false })
            };
        }

        private sealed class TestEntity
        {
            [DigitoVerificador(1)]
            public int Id { get; set; }

            [DigitoVerificador(2)]
            public string Name { get; set; }

            [DigitoVerificador(3)]
            public bool Active { get; set; }
        }
    }
}
